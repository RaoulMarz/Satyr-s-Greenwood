// render as 1200x1600
#include "fanpalm.inc"

background {rgb <0.35,0.325,0.37>}
light_source { <5000,5000,-3000>, rgb 1.2 }
light_source { <-5000,2000,3000>, rgb 0.5 shadowless }

#version 3.7;

#declare Test_Render=false;   // use simplified trees if true
#declare use_iso=false;       // use an isosurface object (HF with function image type otherwise)
#declare Viewpoint=1;

global_settings {
  assumed_gamma 1.0
  max_trace_level 25
  noise_generator 2
}
//-------------------------------------------------------------------------
// This scene uses a non-standard camera set-up.
// (See CAMERA in the included documentation for details.)
// If you are new to POV-Ray, you might want to try a different demo scene.
//-------------------------------------------------------------------------


#if (Viewpoint=1)
  camera {
    //location    <3.7, -0.55, 0.3>
    location    <3.0, 2.3, 0.3>
    direction   y
    sky         z
    up          z
    right       x*image_width/image_height // keep propotions with any aspect ratio
    look_at     <0.0, 0.0, 0.1>
    angle       32
  }
#else
  camera {
    location    <7, 14, 7>
    direction   y
    sky         z
    up          z
    right       x*image_width/image_height // keep propotions with any aspect ratio
    look_at     <0.0, 0.0, 0.0>
    angle       32
  }
#end

light_source {
  <1.0, 4.0, 1.3>*100000
  color rgb <1.5, 1.4, 1.1>
}

// -------- Sky -------------
sphere {
  0,1
  texture {
    pigment {
      gradient z
      color_map {
        [0.03 color rgb <0.55, 0.65, 1.0> ]
        [0.12 color rgb <0.30, 0.40, 1.0> ]
      }
    }
    finish { ambient 1 diffuse 0 }
  }
  scale 1000
  no_shadow
  hollow on
}

// -------- random seeds for the Trees -------------
#declare Seed=seed(2);
#declare Seed2=seed(1);


// -------- Tree textures -------------
#if (Test_Render)
  // simple textures for test renders
  #declare T_Wood=
  texture {
    pigment { color rgb <1, 0, 0> }
  }

  #declare T_Tree=
  texture {
    pigment { color rgb <1, 0, 0> }
  }

#else
  #declare T_Wood=
  texture {
    pigment { color rgb <0.4, 0.2, 0.05> }
    finish {
      specular 0.3
      diffuse 0.5
    }
    normal {
      bozo 0.6
      scale <0.1, 0.1, 0.5>
    }
  }

  #declare T_Wood_S1=
  texture {
    pigment { color rgb <0.42, 0.275, 0.05> }
    finish {
      specular 0.265
      diffuse 0.61
    }
    normal {
      bozo 0.64
      scale <0.1, 0.14, 0.5>
    }
  }

  #declare T_Tree=
  texture {
    pigment {
      agate
      color_map {
        [0.77 color rgbt 1]
        [0.77 color rgb <0.2, 0.5, 0.10> ]
        [0.85 color rgb <0.4, 0.6, 0.15> ]
        [0.97 color rgb <0.4, 0.6, 0.15> ]
        [0.99 color rgb <0.4, 0.2, 0.05> ]
      }
      scale 0.5
      warp { turbulence 0.4 }
    }
    finish {
      diffuse 0.5
      brilliance 1.5
      ambient 0.07
    }
    normal {
      wrinkles 0.6
      scale 0.5
    }
  }
#end

#macro PalmPlant (PHeight, PWidth)
union {
         object { fanpalm_13_stems
                pigment {color rgb 0.9} }
         object { fanpalm_13_leaves
                texture { pigment {color rgb 1}
                          finish { ambient 0.15 diffuse 0.8 }}}
         rotate 90*y }
#end

// -------- Tree macro -------------
#macro TreeA (Size)
  union {
    cylinder { 0, Size*z, Size*0.04 }       // Trunk
    union {                                 // Roots
      cylinder { 0, -Size*0.30*z, Size*0.025 rotate (40+rand(Seed)*20)*x rotate rand(Seed2)*360*z }
      cylinder { 0, -Size*0.25*z, Size*0.020 rotate (40+rand(Seed)*20)*x rotate rand(Seed2)*360*z }
      cylinder { 0, -Size*0.27*z, Size*0.022 rotate (40+rand(Seed)*20)*x rotate rand(Seed2)*360*z }
    }

    union {                                 // Branches
      cylinder {
        0, Size*0.35*z, Size*0.025
        rotate (40+rand(Seed)*35)*x
        rotate rand(Seed2)*360*z
        translate Size*(0.7+0.3*rand(Seed))*z
      }
      cylinder {
        0, Size*0.40*z, Size*0.026
        rotate (40+rand(Seed)*35)*x
        rotate rand(Seed2)*360*z
        translate Size*(0.7+0.3*rand(Seed))*z
      }
      cylinder {
        0, Size*0.27*z, Size*0.022
        rotate (40+rand(Seed)*35)*x
        rotate rand(Seed2)*360*z
        translate Size*(0.7+0.3*rand(Seed))*z
      }
    }

    #if (Test_Render)                       // Foliage
      sphere {
        Size*z, Size*(0.4+rand(Seed)*0.1525)
        scale <rand(Seed)*0.5+0.5, rand(Seed)*0.5+0.5, 1>
        texture { T_Tree scale Size translate rand(Seed)*6 }
      }
    #else
      union {
        sphere {
          Size*z, Size*(0.4+rand(Seed)*0.165)
          scale <rand(Seed)*0.5+0.5, rand(Seed)*0.5+0.5, 1>
          texture { T_Tree scale Size translate rand(Seed)*6 }
        }
        sphere {
          Size*z, Size*(0.3+rand(Seed)*0.1575)
          scale <rand(Seed)*0.5+0.5, rand(Seed)*0.5+0.5, 1>
          texture { T_Tree scale Size translate rand(Seed)*6 }
        }
        sphere {
          Size*z, Size*(0.2+rand(Seed)*0.15)
          scale <rand(Seed)*0.5+0.5, rand(Seed)*0.5+0.5, 1>
          texture { T_Tree scale Size translate rand(Seed)*6 }
        }
        sphere {
          Size*z, Size*(0.3+rand(Seed)*0.15)
          scale <rand(Seed)*0.5+0.5, rand(Seed)*0.5+0.5, 1>
          texture { T_Tree scale Size translate rand(Seed)*6 }
        }
        sphere {
          Size*z, Size*(0.2+rand(Seed)*0.15)
          scale <rand(Seed)*0.5+0.5, rand(Seed)*0.5+0.5, 1>
          texture { T_Tree scale Size translate rand(Seed)*6 }
        }
        sphere {
          Size*z, Size*(0.3+rand(Seed)*0.15)
          scale <rand(Seed)*0.5+0.5, rand(Seed)*0.5+0.5, 1>
          texture { T_Tree scale Size translate rand(Seed)*6 }
        }
      }
    #end

    texture { T_Wood scale Size }
  }
#end

// -------- Terrain textures -------------
#declare T_Sand=
texture {
  pigment { color rgb <1.1, 0.7, 0.3> }
  finish {
    specular 0.06
    ambient <0.8, 0.9, 1.4>*0.1
  }
  normal {
    granite 0.3
    scale 0.1
  }
}

#declare T_Grass=
texture {
  pigment { color rgb <0.5, 1.15, 0.3> }
  finish {
    specular 0.1
    diffuse 0.3
    brilliance 1.6
    ambient <0.8, 0.9, 1.4>*0.03
  }
  normal {
    granite 0.5
    accuracy 0.01
    scale 0.12
  }
}

#declare T_Rock=
texture {
  pigment {
    agate
    color_map {
      [0.2 color rgb <0.55, 0.50, 0.50> ]
      [0.6 color rgb <0.75, 0.50, 0.60> ]
      [1.0 color rgb <0.70, 0.60, 0.60> ]
    }
    scale 0.2
    warp { turbulence 0.5 }
  }
  finish {
    specular 0.2
    diffuse 0.4
    ambient <0.8, 0.9, 1.4>*0.06
  }
  normal {
    granite 0.6
    scale 0.1
  }
}

#declare T_Terrain=
texture {
  slope { -z*0.115 altitude z }
  texture_map {
    [0.035 T_Rock  ]
    [0.9 T_Sand  ]
    [0.135 T_Grass ]
  }
  translate -0.05*z
}

#declare fnPig=   // Terrain shape function
  function{
    pigment {
      agate
      color_map {
        [0.0 color rgb 0.0]
        [0.3 color rgb 0.2]
        [0.7 color rgb 0.8]
        [1.0 color rgb 1.0]
      }
      warp { turbulence 0.015 }
      scale 4
      translate <1.8, -6.3, 0>
    }
  }

  #declare Terrain_Obj=
  isosurface {

    function { z-fnPig(x, -y, 0).gray*0.3 }

    max_gradient 1.715
    //evaluate 1, 10, 0.99    // for evaluating max_gradient
    accuracy 0.03

    contained_by { box { <-3, -3, -0.1>, <3, 3, 0.355> } }

  }


object {
  Terrain_Obj
  texture { T_Terrain }
}


// -------- Placing the trees -------------

#declare Spacing=0.24;
#declare Cnt=0;

#declare PALM_HEIGHT = fanpalm_13_height * 1.3;
#declare PALM_WIDTH = PALM_HEIGHT*1600/1200;

#declare PosX=-3;

#while (PosX < 3)

  #declare PosY=-3;

  #while (PosY < 3)

    // trace function
    #declare randTreeChoice = rand(Seed);
    #declare Norm = <0, 0, 0>;
    #declare Start = <PosX+(rand(Seed)-0.5)*Spacing, PosY+(rand(Seed)-0.5)*Spacing, 1.0>;
    #declare Pos = trace (
                  Terrain_Obj,     // object to test
                  Start,           // starting point
                  -z,              // direction
                  Norm );          // normal


    #if (Norm.x != 0 | Norm.y != 0 | Norm.z != 0)   // if intersection is found, normal differs from 0
      #if ((vdot(Norm, z)>0.85) & (Pos.z < 0.25))
      // criteria for placing trees: not too steep and not too high
        object {
	  #if (randTreeChoice <= 0.1135)
		PalmPlant (PALM_HEIGHT * 0.55, PALM_WIDTH * 0.6)
	  #else
          	TreeA (0.05+rand(Seed)*0.0185)
	  #end
          translate Pos
        }
        #declare Cnt=Cnt+1;
      #end
    #end

    #declare PosY=PosY+Spacing;

  #end

  #declare PosX=PosX+Spacing;
#end

#debug concat("Placed ", str(Cnt,0,0), " Trees\n")
