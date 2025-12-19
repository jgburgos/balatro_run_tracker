// @gips_version=1 @coord=none @filter=off

uniform float range = 1.0;      // @min=0.01 @max=10
uniform float time = 1.0;      // @min=0.01 @max=100

float hue(float s, float t, float h)
{
	float hs = mod(h, 1.)*6.;
	if (hs < 1.) return (t-s) * hs + s;
	if (hs < 3.) return t;
	if (hs < 4.) return (t-s) * (4.-hs) + s;
	return s;
}

vec4 RGB(vec4 c)
{
	if (c.y < 0.0001)
		return vec4(vec3(c.z), c.a);

	float t = (c.z < .5) ? c.y*c.z + c.z : -c.y*c.z + (c.y+c.z);
	float s = 2.0 * c.z - t;
	return vec4(hue(s,t,c.x + 1./3.), hue(s,t,c.x), hue(s,t,c.x - 1./3.), c.w);
}

vec4 HSL(vec4 c)
{
	float low = min(c.r, min(c.g, c.b));
	float high = max(c.r, max(c.g, c.b));
	float delta = high - low;
	float sum = high+low;

	vec4 hsl = vec4(.0, .0, .5 * sum, c.a);
	if (delta == .0)
		return hsl;

	hsl.y = (hsl.z < .5) ? delta / sum : delta / (2.0 - sum);

	if (high == c.r)
		hsl.x = (c.g - c.b) / delta;
	else if (high == c.g)
		hsl.x = (c.b - c.r) / delta + 2.0;
	else
		hsl.x = (c.r - c.g) / delta + 4.0;

	hsl.x = mod(hsl.x / 6., 1.);
	return hsl;
}

vec4 run(vec2 texture_coords) {
    vec4 tex = pixel(texture_coords);
	vec2 uv = texture_coords;

    vec2 adjusted_uv = uv - vec2(0.5, 0.5);
    adjusted_uv.x = adjusted_uv.x; //adjusted_uv.x*texture_details.b/texture_details.a;

    float low = min(tex.r, min(tex.g, tex.b));
    float high = max(tex.r, max(tex.g, tex.b));
	float delta = min(high, max(0.5, 1. - low));

    float fac = max(min(2.*sin((length(90.*adjusted_uv) + range*2.) + 3.*(1.+0.8*cos(length(113.1121*adjusted_uv) - range*3.121))) - 1. - max(5.-length(90.*adjusted_uv), 0.), 1.), 0.);
    vec2 rotater = vec2(cos(range*0.1221), sin(range*0.3512));
    float angle = dot(rotater, adjusted_uv)/(length(rotater)*length(adjusted_uv));
    float fac2 = max(min(5.*cos(time*0.3 + angle*3.14*(2.2+0.9*sin(range*1.65 + 0.2*time))) - 4. - max(2.-length(20.*adjusted_uv), 0.), 1.), 0.);
    float fac3 = 0.3*max(min(2.*sin(range*5. + uv.x*3. + 3.*(1.+0.5*cos(range*7.))) - 1., 1.), -1.);
    float fac4 = 0.3*max(min(2.*sin(range*6.66 + uv.y*3.8 + 3.*(1.+0.5*cos(range*3.414))) - 1., 1.), -1.);

    float maxfac = max(max(fac, max(fac2, max(fac3, max(fac4, 0.0)))) + 2.2*(fac+fac2+fac3+fac4), 0.);

    tex.r = tex.r-delta + delta*maxfac*0.3;
    tex.g = tex.g-delta + delta*maxfac*0.3;
    tex.b = tex.b + delta*maxfac*1.9;
    tex.a = min(tex.a, 0.3*tex.a + 0.9*min(0.5, maxfac*0.1));

vec4 origin = pixel(texture_coords);

// First compute the blended result (what you actually see)
vec3 out_rgb = origin.rgb + tex.rgb * tex.a;

// --- Foil grade (cooler/darker), applied to final output ---
vec3 foil_tint = vec3(0.75, 0.85, 0.80); // cooler multiplier (more blue)
float foil_dark = 0.50;                  // overall darkening (<1 darker)
float foil_strength = 0.70;              // 0..1 strength

vec3 graded = out_rgb * foil_tint * foil_dark;
out_rgb = mix(out_rgb, graded, foil_strength);

return vec4(out_rgb, origin.a);
    }
