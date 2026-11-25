shader_type spatial;
render_mode unshaded;

uniform vec4 BaseColor: source_color;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	
	float fresnel = pow(clamp(dot(VIEW, NORMAL), 0.0, 1.0), 10.0);
	
	vec3 finalColor = BaseColor.rgb;
	
	//OUTPUT
	
	ALBEDO = finalColor;
	ALPHA = fresnel;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
