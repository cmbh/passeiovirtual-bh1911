
MeshBaker is a tool for combining many meshes into one big mesh and creating texture atlases 
from all the textures used by the source meshes. OBJECTS TO BE COMBINED DO NOT NEED THE SAME
MATERIAL OR EVEN THE SAME SHADER. Meshbaker will create empty textures and resize copies of
textures if needed to build the atlases. Meshbaker does not touch the source assets 
(other than setting the read flag on textures). It bakes the source
assets into new assets which can be used in your scene for high performance scene loading.

QUICKSTART

  	MeshBaker is best used in the Unity Editor (see below for runtime use)
  
	Create a new MeshBaker object in your scene under the Game Object menu.
		Game Object
		   -> Create Other
		          -> Mesh Baker
		                 -> MeshBaker
	Follow the instructions in the inspector.
	
	See "Manual.pdf" for more information.

SUPPORT
	
	Please post to the forums at forum.unity.com
	email: ian.deane@gmail.com
