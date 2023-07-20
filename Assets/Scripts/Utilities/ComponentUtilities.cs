using System;
using UnityEngine;

namespace UserInterface
{
    public static class ComponentUtilities
    {
        public static T FindFirstComponentInAncestor<T>(this Transform transform,
            Action<string> tracing = null)
            where T : Component
        {
            if (tracing == null)
            {
                tracing = Debug.LogError;
            }

            var name = transform.name;

            while (transform.parent != null)
            {
                var component = transform.parent.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }

                transform = transform.parent;
            }

            tracing($"[{name}] The ancestors of {name} do not contain a {typeof(T).Name} component.");
            return null;
        }
    }
}