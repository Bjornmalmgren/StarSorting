shader_type spatial;
//render_mode unshaded;
uniform vec3 BaseColor: source_color;
uniform float Roughness;
uniform float EmissionStrength;
uniform float offset;
uniform float falloff;

varying vec3 normalWS;
varying vec3 positionWS;
varying float gradient;
void vertex() {
	// Called for every vertex the material is visible on.
	
	normalWS = normalize((MODEL_MATRIX * vec4(NORMAL, 0.0)).xyz);
	positionWS = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
	// Called for every pixel the material is visible on.
	
	gradient = pow(1.0 - clamp(positionWS.y * falloff + offset, 0.0, 1.0), 2.0);
	//OUTPUT
	
	ALBEDO = BaseColor;
	ALPHA = gradient;
	ROUGHNESS = Roughness;
	EMISSION = BaseColor * EmissionStrength;
	
}

void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.

float specular = clamp(dot(reflect(normalize(VIEW) * vec3(-1.0), NORMAL), normalize(LIGHT)), 0.0, 1.0);
specular = pow(specular, 1000.0) * 10.0;

SPECULAR_LIGHT += specular * ATTENUATION * LIGHT_COLOR * SPECULAR_AMOUNT * 10.0;

}
