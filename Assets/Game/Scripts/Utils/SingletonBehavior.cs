using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Utils
{
    public abstract class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T instance;

        public static T Instance { get => instance; set => instance = value; }

        private void Start()
        {
            Instance = FindObjectOfType<T>();
        }
    }
}
