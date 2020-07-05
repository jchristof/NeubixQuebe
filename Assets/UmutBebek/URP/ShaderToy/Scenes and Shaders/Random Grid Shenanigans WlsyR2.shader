﻿Shader "UmutBebek/URP/ShaderToy/Random Grid Shenanigans WlsyR2"
{
    Properties
    {
        _Channel0("Channel0 (RGB)", 2D) = "" {}
        _Channel1("Channel1 (RGB)", 2D) = "" {}
        _Channel2("Channel2 (RGB)", 2D) = "" {}
        _Channel3("Channel3 (RGB)", 2D) = "" {}
        [HideInInspector]iMouse("Mouse", Vector) = (0,0,0,0)
            /*_Iteration("Iteration", float) = 1
            _NeighbourPixels("Neighbour Pixels", float) = 1
            _Lod("Lod",float) = 0
            _AR("AR Mode",float) = 0*/

TARGET_COUNT("TARGET_COUNT", float) = 15
GRID_CELL_SIZE("GRID_CELL_SIZE", float) = 0.1

    }

        SubShader
        {
            // With SRP we introduce a new "RenderPipeline" tag in Subshader. This allows to create shaders
            // that can match multiple render pipelines. If a RenderPipeline tag is not set it will match
            // any render pipeline. In case you want your subshader to only run in LWRP set the tag to
            // "UniversalRenderPipeline"
            Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
            LOD 300

            // ------------------------------------------------------------------
            // Forward pass. Shades GI, emission, fog and all lights in a single pass.
            // Compared to Builtin pipeline forward renderer, LWRP forward renderer will
            // render a scene with multiple lights with less drawcalls and less overdraw.
            Pass
            {
            // "Lightmode" tag must be "UniversalForward" or not be defined in order for
            // to render objects.
            Name "StandardLit"
            //Tags{"LightMode" = "UniversalForward"}

            //Blend[_SrcBlend][_DstBlend]
            //ZWrite Off ZTest Always
            //ZWrite[_ZWrite]
            //Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //do not add LitInput, it has already BaseMap etc. definitions, we do not need them (manually described below)
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            float4 _Channel0_ST;
            TEXTURE2D(_Channel0);       SAMPLER(sampler_Channel0);
            float4 _Channel1_ST;
            TEXTURE2D(_Channel1);       SAMPLER(sampler_Channel1);
            float4 _Channel2_ST;
            TEXTURE2D(_Channel2);       SAMPLER(sampler_Channel2);
            float4 _Channel3_ST;
            TEXTURE2D(_Channel3);       SAMPLER(sampler_Channel3);

            float4 iMouse;
float TARGET_COUNT;
float GRID_CELL_SIZE;

/*float _Lod;
float _Iteration;
float _NeighbourPixels;
float _AR;*/

struct Attributes
{
    float4 positionOS   : POSITION;
    float2 uv           : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv                       : TEXCOORD0;
    float4 positionCS               : SV_POSITION;
    float4 screenPos                : TEXCOORD1;
};

Varyings LitPassVertex(Attributes input)
{
    Varyings output;

    // VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space)
    // Our compiler will strip all unused references (say you don't use view space).
    // Therefore there is more flexibility at no additional cost with this struct.
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

    // TRANSFORM_TEX is the same as the old shader library.
    output.uv = TRANSFORM_TEX(input.uv, _Channel0);
    // We just use the homogeneous clip position from the vertex input
    output.positionCS = vertexInput.positionCS;
    output.screenPos = ComputeScreenPos(vertexInput.positionCS);
    return output;
}

#define FLT_MAX 3.402823466e+38
#define FLT_MIN 1.175494351e-38
#define DBL_MAX 1.7976931348623158e+308
#define DBL_MIN 2.2250738585072014e-308

 #define iTimeDelta unity_DeltaTime.x
// float;

#define iFrame ((int)(_Time.y / iTimeDelta))
// int;

#define clamp(x,minVal,maxVal) min(max(x, minVal), maxVal)

float mod(float a, float b)
{
    return a - floor(a / b) * b;
}
float2 mod(float2 a, float2 b)
{
    return a - floor(a / b) * b;
}
float3 mod(float3 a, float3 b)
{
    return a - floor(a / b) * b;
}
float4 mod(float4 a, float4 b)
{
    return a - floor(a / b) * b;
}




#define RED float3 ( 1.0 , 0.0 , 0.0 ) 
#define GREEN float3 ( 0.0 , 1.0 , 0.0 ) 
#define BLUE float3 ( 0.0 , 0.0 , 1.0 ) 

float2 getGridPosition(in float2 uv)
 {
     return float2 ((uv.x / GRID_CELL_SIZE) , (uv.y / GRID_CELL_SIZE));
 }

half4 LitPassFragment(Varyings input) : SV_Target  {
half4 fragColor = half4 (1 , 1 , 1 , 1);
float2 fragCoord = ((input.screenPos.xy) / (input.screenPos.w + FLT_MIN)) * _ScreenParams.xy;
// Normalized frag coordinates 
float2 uv = (fragCoord - (0.5 * _ScreenParams.xy)) / _ScreenParams.y;

float2 gridBoundUV = getGridPosition(uv);

float2 cellBoundUV = gridBoundUV - round(gridBoundUV);

float redIntensity = 0.0;
float blueIntensity = 0.0;

for (int targetIndex = 0; targetIndex < TARGET_COUNT; ++targetIndex)
 {
    float f_targetIndex = float(targetIndex);

    float trigOffset = (PI / float(TARGET_COUNT)) * f_targetIndex;
    float2 targetPosition = float2 (sin(_Time.y + trigOffset) * 0.51 + tan(f_targetIndex + trigOffset) , cos(_Time.y + trigOffset) * 0.1 + sin(f_targetIndex + trigOffset));
    float2 gridBoundTargetPosition = getGridPosition(targetPosition);
    float2 edgeBoundPosition = float2 (gridBoundTargetPosition.x , gridBoundTargetPosition.y);

    // change the op between the lengths to subtraction for some extreme strobe effects 
   float distanceToTarget = length(gridBoundUV - round(gridBoundTargetPosition)) + length((gridBoundUV)-(edgeBoundPosition));

    redIntensity += length(GRID_CELL_SIZE / (distanceToTarget * 9.5) / cellBoundUV) * GRID_CELL_SIZE;

}

for (int targetIndex = 0; targetIndex < TARGET_COUNT; ++targetIndex)
 {
    float f_targetIndex = float(targetIndex);

    float trigOffset = (PI / float(TARGET_COUNT)) * f_targetIndex;

    float2 targetPosition = float2 (sin(_Time.y + trigOffset) * 0.51 + sin(f_targetIndex + trigOffset) , tan(_Time.y + trigOffset) * 0.1 + sin(f_targetIndex + trigOffset));
    float2 gridBoundTargetPosition = getGridPosition(targetPosition);
    float2 edgeBoundPosition = float2 (gridBoundTargetPosition.x , gridBoundTargetPosition.y);

    float distanceToTarget = length(gridBoundUV - round(gridBoundTargetPosition)) + length((gridBoundUV)-(edgeBoundPosition));

     blueIntensity += length(GRID_CELL_SIZE / (distanceToTarget * 15.5) / cellBoundUV) * GRID_CELL_SIZE;

 }


float3 col = float3 (smoothstep(0.2, 1.0, redIntensity + blueIntensity),
    smoothstep(0.2, 1.0, redIntensity + blueIntensity),
    smoothstep(0.2, 1.0, redIntensity + blueIntensity));

col += redIntensity * GREEN;
 col += blueIntensity * BLUE;

 // Output to screen 
fragColor = float4 (col , 1.0);
return fragColor - 0.1;
}

//half4 LitPassFragment(Varyings input) : SV_Target
//{
//    [FRAGMENT]
//    //float2 uv = input.uv;
//    //SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, uv + float2(-onePixelX, -onePixelY), _Lod);
//    //_ScreenParams.xy 
//    //half4 color = half4(1, 1, 1, 1);
//    //return color;
//}
ENDHLSL
}
        }
}