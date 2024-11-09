using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LowEndGames.ObjectTagSystem
{
    /// <summary>
    /// defines a Tag, will have a corresponding enum value generated
    /// </summary>
    [CreateAssetMenu(menuName = "ObjectTags System/Tag")]
    public class ObjectTag : ScriptableObject
    {
        [Tooltip("objects must pass this filter for this tag to be added")]
        public List<TagsFilter> Filters = new();
        
        [Tooltip("tag actions performed on the owner when this tag is added")]
        public List<TagAction> ActionsOnAdded = new(); 
        
        [Tooltip("tag actions performed on the owner when this tag is removed")]
        public List<TagAction> ActionsOnRemoved = new(); 
        
        [Tooltip("behaviours which are active when this tag is on an object")]
        public List<TagBehaviourSettings> Behaviours = new();

        public string EnumStringValue;

        public string GetEnumValueName()
        {
            var trimmed = name.Replace(" ", "").Split('.').Last(); // only take last string, to support asset names like Tag.Whatever
            
            var result = Regex.Replace(trimmed, @"[^A-Za-z0-9]+", "")
                .Replace(".", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "");
            
            #if UNITY_EDITOR

            if (EnumStringValue != result)
            {
                EnumStringValue = result;
                UnityEditor.EditorUtility.SetDirty(this);
            }
            
            #endif

            return result;
        }
    }
}