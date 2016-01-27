Shader "MotionBlur/MotionBlur"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}
		
		BindChannels 
		{
	        Bind "Color", color
	        Bind "Vertex", vertex
	        Bind "TexCoord", texcoord
    	}
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass 
		{
			Lighting Off
			
			// Texture
			SetTexture [_MainTex] 
			{
				combine texture * primary
			} 
			SetTexture [_MainTex] 
			{
                constantColor [_Color]
                combine previous * constant
            }
		}
	}
}
