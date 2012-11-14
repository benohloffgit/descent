== TRIPLANAR TEXTURE PROJECTION SHADERS ==
Copyright(c) Broken Toy Games, 2011. Do not redistribute.
http://www.brokentoygames.com

== Turn meshes into matter and terrain! ==
* These shaders assign textures to arbitrary meshes and do not require UV wrapping. Create dynamic matter from your models, limited only by your modeling skills.
* Take your environments to the next level and free yourself of splat textures with the terrain shaders, which put ground on the ground, walls on the walls and ceilings on the ceiling. 
* If more terrain detail is needed, combine up to five textures using vertex colors. The details stick to the ground and do not spill on the walls.

FIX FOR MACS: A possible fix for Macs is to manually disable the mipmapping option when importing your textures, as some graphic cards are known to display the -entire- resulting texture.

Triplanar projection texture shaders are a completely different beast than Unity projectors. They are meant to replace texturing completely, by wrapping a 3D mesh from all angles with a seamless texture, without stretching or bleeding on other objects. A specialized version is used to turn 3D meshes into terrain, where one texture is aplied on the top side, like grass, and the rest of the mesh (cliffs, overhangs, ceilings, etc.) uses a second texture. 

== Importing a Model ==
Models need Normals to apply textures on, and Tangents for the bumpmapping. If the textures don't apply correctly, set the Nomals to Calculate and the Smoothing Angle to 89 degrees when importing into the Unity Editor.

== World Projection Shaders ==
The texture uses the same coordinates for all objects, which is useful to have multiple meshes look seamless when touching one another.

== Local Projection Shaders ==
The texture sticks to the model, which can be moved, rotated and scaled at runtime.
* Known issue: Uniform scaling keeps the texture tiling of the original size. This can be fixed with a .01% (.0001 per unit) scale offset on one axis.

== Multi Texture Shaders == 
These shaders project a texture on the top of a mesh and use a different texture to cover the sides and bottom, effectively turning it into a primitive form of terrain.

== Vertex Blend Terrain Shaders ==
(* Requires a Shader Model 3.0 graphics card)
Advanced terrain which adds four extra textures to the ground and blends them using the vertex color of the mesh.

== Painting Vertex Colors ==
Vertex colors can be painted on meshes using 3D modeling tools such as Blender, 3DS Max or Maya, or Unity Editor extensions such as VertexPainter, available on the Asset Store.