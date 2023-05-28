using UnityEngine;

public static class Util
{
    public static bool FindAlongChild(this Transform tf, string name, out Transform result)
    {
        for (int i = 0; i < tf.childCount; i++)
        {
            string a = tf.GetChild(i).name;
            if (tf.GetChild(i).name.Equals(name))
            {
                result = tf.GetChild(i);
                return true;
            }

            if (tf.GetChild(i).FindAlongChild(name, out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }
}