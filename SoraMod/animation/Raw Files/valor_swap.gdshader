shader_type canvas_item;

// These create color pickers in your Godot Inspector!
uniform vec4 original_color : source_color;
uniform vec4 new_color : source_color;

// This controls how strict the color matching is
uniform float tolerance : hint_range(0.0, 1.0) = 0.15;

void fragment() {
    // Get the current pixel color from Sora's sprite
    vec4 current_pixel = texture(TEXTURE, UV);
    
    // Check if the pixel's color is close to the original outfit color
    if (distance(current_pixel.rgb, original_color.rgb) < tolerance) {
        // Swap it to the new color, but keep the original transparency
        COLOR = vec4(new_color.rgb, current_pixel.a);
    } else {
        // Otherwise, draw the normal pixel
        COLOR = current_pixel;
    }
}