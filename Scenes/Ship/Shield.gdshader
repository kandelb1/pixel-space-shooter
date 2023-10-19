shader_type canvas_item;

uniform float time : hint_range(0, 10) = 0.0;
uniform bool is_active = false;

uniform vec2 ripple_origin = vec2(0, 1);
uniform float ripple_radius : hint_range(0, 1) = 0.2;
uniform float ripple_strength : hint_range(0, 1) = 0.5;
uniform float ripple_speed : hint_range(1, 20) = 10.0;
uniform float distortion : hint_range(0, 1) = 0.1;

uniform vec2 frame_size = vec2(64.0, 64.0);

void fragment() {
  if(!is_active){
    COLOR = texture(TEXTURE, UV);
  }else{
    vec2 normalized_uv = fract(UV);
    vec2 remapped_ripple_origin = ripple_origin * frame_size;
    
    vec2 delta = normalized_uv - ripple_origin;
    float distance = length(delta);
      
    // Adjust the ripple effect with distortion
    float ripple = ripple_strength * sin(TIME * ripple_speed - distance * 8.5);
    delta *= ripple * distortion;

    // Apply distortion to UV coordinates
    vec2 distorted_uv = clamp(normalized_uv - delta, vec2(0, 0), vec2(1, 1));

    // Sample the original texture using distorted UV coordinates
    vec4 original_color = texture(TEXTURE, distorted_uv);

    // Mix the original color with the ripple effect
    COLOR = original_color + vec4(ripple, ripple, ripple, 0); 
  }
}
