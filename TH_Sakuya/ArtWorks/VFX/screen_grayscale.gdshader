shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec4 col = texture(screen_texture, SCREEN_UV);
	float g = dot(col.rgb, vec3(0.299, 0.587, 0.114));
	COLOR = vec4(vec3(g), col.a);
}

