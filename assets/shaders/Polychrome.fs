// @gips_version=1 @coord=none @filter=off

uniform float range = 1.0;      // @min=0.01 @max=10
uniform float time = 10.0;      // @min=0.01 @max=10


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

    float low = min(tex.r, min(tex.g, tex.b));
    float high = max(tex.r, max(tex.g, tex.b));
	float delta = high - low;

	float saturation_fac = 1. - max(0., 0.05*(1.1-delta));

	vec4 hsl = HSL(vec4(tex.r*saturation_fac, tex.g*saturation_fac, tex.b, tex.a));

	float t = time*2.221 + time;
	vec2 floored_uv = uv;//(floor((uv*texture_details.ba)))/texture_details.ba;
    vec2 uv_scaled_centered = (floored_uv - 0.5) * 50.;
	
	vec2 field_part1 = uv_scaled_centered + 50.*vec2(sin(-t / 143.6340), cos(-t / 99.4324));
	vec2 field_part2 = uv_scaled_centered + 50.*vec2(cos( t / 53.1532),  cos( t / 61.4532));
	vec2 field_part3 = uv_scaled_centered + 50.*vec2(sin(-t / 87.53218), sin(-t / 49.0000));

    float field = (1.+ (
        cos(length(field_part1) / 19.483) + sin(length(field_part2) / 33.155) * cos(field_part2.y / 15.73) +
        cos(length(field_part3) / 27.193) * sin(field_part3.x / 21.92) ))/2.;

    float res = (.5 + .5* cos( (range) * 2.612 + ( field + -.5 ) *3.14));
	hsl.x = hsl.x+ res + time*0.04;
	hsl.y = min(0.6,hsl.y+0.5);

    tex.rgb = RGB(hsl).rgb;

	if (tex[3] < 0.7)
		tex[3] = tex[3]/3.;

    return tex;
}
