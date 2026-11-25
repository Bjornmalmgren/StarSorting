shader_type spatial;
//render_mode unshaded;


uniform vec4 BaseColor: source_color;
uniform sampler2D BaseColorTexture: source_color;
uniform bool UseColorTexture;
uniform bool UseAlpha;
uniform float Metallic: hint_range(0.0, 1.0, 0.001);
uniform sampler2D MetallicTexture: source_color;
uniform bool UseMetallicTexture;
uniform float Roughness: hint_range(0.0, 1.0, 0.001);
uniform bool UseSpecular;
uniform sampler2D RoughnessTexture: source_color;
uniform bool UseRoughnessTexture;
uniform vec3 EdgeColor: source_color;
uniform vec3 TopColor: source_color;
uniform vec3 BottomColor: source_color; 
uniform vec3 Sky: source_color;
uniform vec3 Ground: source_color; 
uniform	sampler2D screen: hint_screen_texture;

varying vec3 normalWS;

void vertex() {
	// Called for every vertex the material is visible on.
	
	normalWS = normalize((MODEL_MATRIX * vec4(NORMAL, 0.0)).xyz);
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	
	float fresnel = clamp(dot(VIEW, NORMAL), 0.0, 1.0);
	float topFacesDiffuse = pow(clamp(dot(vec3(0.0, 1.0, 0.0 ), normalWS), 0.0, 1.0), 2.0);
	float bottomFacesDiffuse = pow(clamp(dot(vec3(0.0, -1.0, 0.0 ), normalWS), 0.0, 1.0), 2.0);
	
	vec3 ColorTex = texture(BaseColorTexture, UV).rgb;
	vec3 RoughnessTex = texture(RoughnessTexture, UV).rgb;
	vec3 MetallicTex = texture(RoughnessTexture, UV).rgb;
	
	vec3 Edge = pow(1.0 - fresnel, 3.0) * EdgeColor;
	vec3 TopDiffuse = topFacesDiffuse * TopColor;
	vec3 BottomDiffuse = bottomFacesDiffuse * BottomColor;
	
	//OUTPUT
	if (UseColorTexture == true) {
		ALBEDO = ColorTex;
	} else {
		ALBEDO = BaseColor.rgb;
	}
	
	if (UseRoughnessTexture == true) {
		ROUGHNESS = clamp((pow(RoughnessTex.r, 0.2)), 0.0, 1.0);
	} else {
		ROUGHNESS = pow(Roughness, 0.2);
	}
	
	if (UseMetallicTexture == true) {
		METALLIC = MetallicTex.r;
	} else {
		METALLIC = Metallic;
	}
	
	EMISSION = Edge + TopDiffuse + BottomDiffuse;
}

void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.



float specular = clamp(dot(reflect(normalize(VIEW) * vec3(-1.0), NORMAL), normalize(LIGHT)), 0.0, 1.0);
specular = pow(specular, mix(100.0, 1.0, ROUGHNESS)) * mix(1.0, 0.1, ROUGHNESS);

vec3 metallicColor = mix(vec3(1.0), ALBEDO, METALLIC);

DIFFUSE_LIGHT += clamp(dot(NORMAL, LIGHT), 0.0, 1.0) * ATTENUATION * LIGHT_COLOR * (1.0 - Metallic);

if (UseSpecular == true) {
	SPECULAR_LIGHT += specular * ATTENUATION * LIGHT_COLOR * SPECULAR_AMOUNT * metallicColor;
}

}
