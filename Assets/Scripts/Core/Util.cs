using UnityEngine;

namespace RPG.Core
{
    public static class Util
    {
        public static bool FindAlongChild(this Transform tf, string name, out Transform result,
            bool perciseSearch = false)
        {
            for (int i = 0; i < tf.childCount; i++)
            {
                string a = tf.GetChild(i).name;
                if (perciseSearch)
                {
                    if (tf.GetChild(i).name.Equals(name))
                    {
                        result = tf.GetChild(i);
                        return true;
                    }
                }
                else
                {
                    if (tf.GetChild(i).name.Contains(name))
                    {
                        result = tf.GetChild(i);
                        return true;
                    }
                }

                if (tf.GetChild(i).FindAlongChild(name, out result, perciseSearch))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}