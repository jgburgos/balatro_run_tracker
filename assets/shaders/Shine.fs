// @gips_version=1 @coord=none @filter=off

uniform float range = 1.0;      // @min=0.01 @max=10

vec4 run(vec2 texture_coords) {
    vec4 tex = pixel(texture_coords);
    vec2 uv = texture_coords;
    
    float low = min(tex.r, min(tex.g, tex.b));
    float high = max(tex.r, max(tex.g, tex.b));
	float delta = high-low -0.1;

    vec2 negative_shine = vec2(0.4);

    float fac = 0.8 + 0.9*sin(11.*uv.x+4.32*uv.y + range*12. + cos(range*5.3 + uv.y*4.2 - uv.x*4.));
    float fac2 = 0.5 + 0.5*sin(8.*uv.x+2.32*uv.y + range*5. - cos(range*2.3 + uv.x*8.2));
    float fac3 = 0.5 + 0.5*sin(10.*uv.x+5.32*uv.y + range*6.111 + sin(range*5.3 + uv.y*3.2));
    float fac4 = 0.5 + 0.5*sin(3.*uv.x+2.32*uv.y + range*8.111 + sin(range*1.3 + uv.y*11.2));
    float fac5 = sin(0.9*16.*uv.x+5.32*uv.y + range*12. + cos(range*5.3 + uv.y*4.2 - uv.x*4.));

    float maxfac = 0.7*max(max(fac, max(fac2, max(fac3,0.0))) + (fac+fac2+fac3*fac4), 0.);

    tex.rgb = tex.rgb*0.5 + vec3(0.4, 0.4, 0.8);

    tex.r = tex.r-delta + delta*maxfac*(0.7 + fac5*0.27) - 0.1;
    tex.g = tex.g-delta + delta*maxfac*(0.7 - fac5*0.27) - 0.1;
    tex.b = tex.b-delta + delta*maxfac*0.7 - 0.1;
    tex.a = tex.a*(0.5*max(min(1., max(0.,0.3*max(low*0.2, delta)+ min(max(maxfac*0.1,0.), 0.4)) ), 0.) + 0.15*maxfac*(0.1+delta));

    vec4 origin = pixel(texture_coords);
    return vec4(origin.rgb + tex.rgb * tex.a, origin.a);
}
