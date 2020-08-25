
float2 RotateUV(float2 uv, float rotationDeg, float aspect)
{
	float2 cuv = fixed2(0.5, 0.5);

	float2 v = uv - cuv;
	v.x *= aspect;
	float r = radians(rotationDeg);
	float cs = cos(r);
	float sn = sin(r);
	uv = float2((v.x * cs - v.y * sn) / aspect, v.x * sn + v.y * cs) + cuv.xy;
	return uv;
}

float AlphaFromRangeToPosition(float3 position, float min, float max)
{
	float l = length(position);
	if (l < min) return 1;
	else if (l > max) return 0;
	else return 1 - ((l - min) / (max - min));
}
