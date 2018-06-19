using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Remote;
using Domain;
using Domain.BehaviourMessages;
using Hyperion.Internal;
using UnityEngine;

namespace Jedium.Utils
{
    static class JediumEx
    {
        #region from GameObjectBox

        /// <summary>
        /// Get Position Vector 3 from GameObject Box
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static Vector3 GetPostionFromBox(this JediumTransformMessage g)
        {
            return new Vector3(g.X, g.Y, g.Z);
        }
        /// <summary>
        /// Get Rotation Vector 4 from GameObject Box
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static Vector4 GetRotationFromBox(this JediumTransformMessage g)
        {
            return new Vector4(g.RotX, g.RotY, g.RotZ, g.RotW);
        }
        /// <summary>
        /// Get Scale Vector 3 from GameObject Box
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static Vector3 GetScaleFromBox(this JediumTransformMessage g)
        {
            return new Vector3(g.ScaleX, g.ScaleY, g.ScaleZ);
        }

        /// <summary>
        /// Check change position box object
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dtTranslate"></param>
        /// <returns></returns>
        public static bool CheckChangeBoxPosition(this JediumTransformMessage a, Transform b, float dtTranslate)
        {
            return (Mathf.Abs(Vector3.Distance(a.GetPostionFromBox(), b.position)) > dtTranslate) ? true : false;
        }
        /// <summary>
        /// Check change scale box object
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dtScale"></param>
        /// <returns></returns>
        public static bool CheckChangeBoxScale(this JediumTransformMessage a, Transform b, float dtScale)
        {
            return (Mathf.Abs(Vector3.Distance(a.GetScaleFromBox(), b.localScale)) > dtScale) ? true : false;
        }
        /// <summary>
        /// Check change rotation box object
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dtAngel"></param>
        /// <returns></returns>
        public static bool CheckChangeBoxRotation(this JediumTransformMessage a, Transform b, float dtAngel)
        {
            Vector4 quaternion = new Vector4(b.rotation.x, b.rotation.y, b.rotation.z, b.rotation.w);

            return (Mathf.Abs(Vector3.Distance(a.GetRotationFromBox(), quaternion)) > dtAngel) ? true : false;
        }
        /// <summary>
        /// Check all change box object state
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dtTranslate"></param>
        /// <param name="dtAngel"></param>
        /// <param name="dtScale"></param>
        /// <returns></returns>
        public static bool CheckChangeGameObjectBox(this JediumTransformMessage a, Transform b, float dtTranslate, float dtAngel,
            float dtScale)
        {
            if (
                CheckChangeBoxPosition(a, b, dtTranslate) ||
                CheckChangeBoxScale(a, b, dtScale) ||
                CheckChangeBoxRotation(a, b, dtAngel)
            )
            {
                return true;
            }

            return false;
        }

        #endregion


        #region FromTrasform

        /// <summary>
        /// Check change position object 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dtTranslate"></param>
        /// <returns></returns>
        public static bool CheckChangeTrasformPosition(this Transform a, Transform b, float dtTranslate)
        {
            return (Mathf.Abs(Vector3.Distance(a.position, b.position)) > dtTranslate) ? true : false;
        }

        /// <summary>
        /// Check change scale object 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dtScale"></param>
        /// <returns></returns>
        public static bool CheckChangeTrasformScale(this Transform a, Transform b, float dtScale)
        {
            return (Mathf.Abs(Vector3.Distance(a.localScale, b.localScale)) > dtScale) ? true : false;
        }

        /// <summary>
        /// Check change Rotation object 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dtAngel"></param>
        /// <returns></returns>
        public static bool CheckChangeTrasformRotation(this Transform a, Transform b, float dtAngel)
        {
            return (Mathf.Abs(Vector3.Distance(a.eulerAngles, b.eulerAngles)) > dtAngel) ? true : false;
        }

        /// <summary>
        /// Boxing all state (position, roation, scale) from Trasform component
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static JediumTransformMessage SetGameObjectFromTrasform(this Transform t)
        {
           //return new JediumTransformMessage
           //    {
           //        X=t.position.x, 
           //        Y=t.position.y,
           //        Z=t.position.z,
           //        RotX = t.rotation.x,
           //        RotY = t.rotation.y,
           //        RotZ = t.rotation.z,
           //        RotW = t.rotation.w,
           //        ScaleX=t.localScale.x,
           //        ScaleY = t.localScale.y,
           //        ScaleZ = t.localScale.z
           //
           //};

            return new JediumTransformMessage(t.position.x,t.position.y,t.position.z,
                t.rotation.x,t.rotation.y,t.rotation.z,t.rotation.w,
                t.localScale.x,t.localScale.y,t.localScale.z);
            
        }

        /// <summary>
        /// Unboxing all state (position, rotation, scale) from Game Object Box 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="box"></param>
        public static void SetTransformFromGameObjectBox(this Transform t, JediumTransformSnapshot box)
        {
            t.position = new Vector3(box.X, box.Y, box.Z);
            t.rotation = new Quaternion(box.RotX, box.RotY, box.RotZ, box.RotW);
            t.localScale = new Vector3(box.ScaleX, box.ScaleY, box.ScaleZ);
        }

        public static void SetTransformFromGameObjectBox(this Transform t, JediumTransformMessage box)
        {
            t.position = new Vector3(box.X, box.Y, box.Z);
            t.rotation = new Quaternion(box.RotX, box.RotY, box.RotZ, box.RotW);
            t.localScale = new Vector3(box.ScaleX, box.ScaleY, box.ScaleZ);
        }

        /// <summary>
        /// Check cahange all state 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="dtTranslate"></param>
        /// <param name="dtAngel"></param>
        /// <param name="dtScale"></param>
        /// <returns></returns>
        public static bool CheckChangeTransform(this Transform a, Transform b, float dtTranslate, float dtAngel,
            float dtScale)
        {
            if (
                CheckChangeTrasformPosition(a, b, dtTranslate) ||
                CheckChangeTrasformScale(a, b, dtScale) ||
                CheckChangeTrasformRotation(a, b, dtAngel)
            )
            {
                return true;
            }

            return false;
        }

        #endregion


        public static string ToHex(this byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }


       

        public static Color ToColor(this SerializableColor scolor)
        {
            return new Color(scolor.R,scolor.G,scolor.B,scolor.A);
        }

        public static SerializableColor FromColor(this Color color)
        {
            return new SerializableColor()
            {
                A = color.a,
                B = color.b,
                G = color.g,
                R = color.r
            };
        }

    }

    public class SerializableColor
    {
        public float R;
        public float G;
        public float B;
        public float A;
    }
}