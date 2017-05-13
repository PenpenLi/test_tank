using UnityEngine;

public class MathUtil {

    /***
     * 确定两个指定点之间的点。参数 f 确定新的内插点相对于参数 pos1 和 pos2 指定的两个端点所处的位置。
     * 参数 f 的值越接近 1.0，则内插点就越接近第一个点（参数 pos1）。
     * 参数 f 的值越接近 0，则内插点就越接近第二个点（参数 pos2）。
     */
    public static Vector2 Interpolate(Vector2 pos1, Vector2 pos2, float f)
    {
        float x = f * (pos1.x - pos2.x) + pos2.x;
        float t = (pos2.x - pos1.x) * (pos2.y - pos1.y);
        float y = 0;
        if (t != 0)
        {
            y = (x - pos1.x) / (pos2.x - pos1.x) * (pos2.y - pos1.y) + pos1.y;
        }
        else
        {
            y = f * (pos1.y - pos2.y) +pos2.y;
        }
        return new Vector2(x, y);
    }

    /***
     *  
     * @return 
     * 弧度转度数
     */
    public static float RadianToAngle(float radian)
	{
		return radian / Mathf.PI* 180;
	}

    /***
	 *  
	 * @return 
	 * 度数转弧度
	 */
    public static float AngleToRadian(float angle)
	{
		return angle / 180 * Mathf.PI;
	}


    /***
	 * 计算由Point1到Point2的角度 
	 * @param point1
	 * @param point2
	 * @return 
	 * 
	 */
    public static float GetAngleTwoPoint(Vector2 point1, Vector2 point2)
	{
		float disX = point1.x - point2.x;
        float disY = point1.y - point2.y;
		return Mathf.Floor(RadianToAngle(Mathf.Atan2(disY , disX)));
	}

    /// <summary>
    /// Convert from top-left based pixel coordinates to bottom-left based UV coordinates.
    /// </summary>

    static public Rect ConvertToTexCoords(Rect rect, int width, int height)
    {
        Rect final = rect;

        if (width != 0f && height != 0f)
        {
            final.xMin = rect.xMin / width;
            final.xMax = rect.xMax / width;
            final.yMin = 1f - rect.yMax / height;
            final.yMax = 1f - rect.yMin / height;
        }
        return final;
    }

}
