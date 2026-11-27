shader_type sky;

uniform sampler2D NoiseTexture: source_color;
uniform sampler2D CloudNoiseTexture: source_color;
uniform sampler2D VoronoiTexture: source_color;
uniform sampler2D BigVoronoiTexture: source_color;
uniform sampler2D WarpTexture: source_color;
uniform float WarpStrength;
uniform float WarpSpeed;

uniform float Brighness = 1.0;
uniform float StarBrighness = 10.0;
uniform float CloudBrigtness = 1.0;
uniform float CLoudAbsorbRed = 1.0;
uniform float CLoudAbsorbGreen = 1.0;
uniform float CloudAbsorbBlue = 1.0;
//uniform sampler3D StarTexture: source_color;

void sky() {
	// Called for every visible pixel in the sky background, as well as all pixels
	// in the radiance cubemap.
	
	// TEMP UVs 
	
	vec2 TopUV = vec2(EYEDIR.r, EYEDIR.b);
	vec2 SidesUV = vec2(SKY_COORDS * 5.0);
	//texture to warp UVs
	vec3 WarpTop = texture(WarpTexture, TopUV ).rgb;
	vec3 WarpSides = texture(WarpTexture, SidesUV).rgb;
	
	WarpTop = sin((WarpTop * 100.0) + TIME * WarpSpeed);
	WarpSides = sin((WarpSides * 100.0) + TIME * WarpSpeed);
	
	vec2 WarpedTopUV = TopUV - WarpTop.rg * WarpStrength; 
	vec2 WarpedSidesUV = SidesUV - WarpSides.rg * WarpStrength;
	
	
	//SAMPLING TEXTURES SO SO SO MANY TEXTURES
	
	vec3 NoiseTop = texture(NoiseTexture, WarpedTopUV).rgb;
	vec3 NoiseSides = texture(NoiseTexture, WarpedSidesUV).rgb;
	
	vec3 ExtraNoiseTop = texture(NoiseTexture, TopUV).rgb;
	vec3 ExtraNoiseSides = texture(NoiseTexture, SidesUV).rgb;
	
	vec3 CloudTop = texture(CloudNoiseTexture, WarpedTopUV).rgb;
	vec3 CloudSides = texture(CloudNoiseTexture, WarpedSidesUV).rgb;
	
	vec3 ExtraCloudTop = texture(CloudNoiseTexture, TopUV).rgb;
	vec3 ExtraCloudSides = texture(CloudNoiseTexture, SidesUV).rgb;
	
	vec3 VoronoiTop = texture(VoronoiTexture, TopUV).rgb;
	vec3 VoronoiSides = texture(VoronoiTexture, SidesUV).rgb;
	
	vec3 BigVoronoiTop = texture(BigVoronoiTexture, TopUV).rgb;
	vec3 BigVoronoiSides = texture(BigVoronoiTexture, SidesUV).rgb;
	
	//MIXING SO THE SKY LOOKS GOOD //REPLACE WITH BETTER METHOD !!
	
	//mixing mask
	float TopMix = dot(EYEDIR, vec3(0.0, -1.0, 0.0));
	TopMix = pow(1.0 - abs(TopMix), 2.0);
	
	//mixing textures
	vec3 NoiseMixed = mix(NoiseTop, NoiseSides, TopMix);
	NoiseMixed = pow(NoiseMixed, vec3(4.0));
	
	vec3 ExtraNoiseMixed = mix(ExtraNoiseTop, ExtraCloudSides, TopMix);
	ExtraNoiseMixed = pow(ExtraNoiseMixed, vec3(4.0));
	
	vec3 CloudMixed = mix(CloudTop, CloudSides, TopMix);
	CloudMixed = pow(CloudMixed, vec3(2.0));
	
	vec3 ExtraCloudMixed = mix(ExtraCloudTop, ExtraCloudSides, TopMix);
	ExtraCloudMixed = pow(ExtraCloudMixed, vec3(2.0));
	
	vec3 VoronoiMixed = mix(VoronoiTop, VoronoiSides, TopMix);
	VoronoiMixed = pow(VoronoiMixed, vec3(20.0));
	vec3 Stars = VoronoiMixed * NoiseMixed * StarBrighness * 200.0;
	
	vec3 BigVoronoiMixed = mix(BigVoronoiTop, BigVoronoiSides, TopMix);
	BigVoronoiMixed = pow(BigVoronoiMixed, vec3(80.0));
	vec3 BigStars = BigVoronoiMixed * StarBrighness * 400.0;
	// COMBINING TEXTURES 
	
	vec3 CloudColor = vec3(pow(CloudMixed.r, CLoudAbsorbRed), pow(CloudMixed.r, CLoudAbsorbGreen), pow(CloudMixed.r, CloudAbsorbBlue));
	vec3 ExtraCloudColor = vec3(pow(ExtraCloudMixed.r, CLoudAbsorbRed), pow(ExtraCloudMixed.r, CLoudAbsorbGreen), pow(ExtraCloudMixed.r, CloudAbsorbBlue));
	
	vec3 StarsColor = Stars * CloudColor;
	vec3 BigStarsColor = BigStars * CloudColor;
	
	vec3 FinalSky = StarsColor + BigStarsColor + max((NoiseMixed * CloudColor), (ExtraNoiseMixed * CloudColor)) + max((CloudColor * CloudBrigtness), (ExtraCloudColor * CloudBrigtness));
	
	
	//OUTPUT
	
	COLOR = FinalSky * Brighness;
	
}
