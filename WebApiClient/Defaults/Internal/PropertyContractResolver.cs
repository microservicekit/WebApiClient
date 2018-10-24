﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using WebApiClient.DataAnnotations;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示属性解析约定
    /// 用于实现DataAnnotations的功能
    /// </summary>
    class PropertyContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 是否camel命名
        /// </summary>
        private readonly bool useCamelCase;

        /// <summary>
        /// 序列化范围
        /// </summary>
        private readonly FormatScope formatScope;

        /// <summary>
        /// 属性解析器
        /// </summary>
        /// <param name="camelCase">是否camel命名</param>
        /// <param name="scope">序列化范围</param>
        public PropertyContractResolver(bool camelCase, FormatScope scope)
        {
            this.useCamelCase = camelCase;
            this.formatScope = scope;
        }

        /// <summary>
        /// 字典Key的CamelCase
        /// </summary>
        /// <param name="dictionaryKey"></param>
        /// <returns></returns>
        protected override string ResolveDictionaryKey(string dictionaryKey)
        {
            var name = base.ResolveDictionaryKey(dictionaryKey);
            return this.useCamelCase ? FormatOptions.CamelCase(name) : name;
        }

        /// <summary>        
        /// 创建Json属性
        /// </summary>
        /// <param name="member">属性</param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var annotations = Annotations.GetAnnotations(member, this.formatScope);

            if (string.IsNullOrEmpty(annotations.AliasName) == false)
            {
                property.PropertyName = annotations.AliasName;
            }
            else if (this.useCamelCase == true)
            {
                property.PropertyName = FormatOptions.CamelCase(property.PropertyName);
            }

            if (string.IsNullOrEmpty(annotations.DateTimeFormat) == false)
            {
                property.Converter = new IsoDateTimeConverter { DateTimeFormat = annotations.DateTimeFormat };
            }

            if (annotations.IgnoreWhenNull == true)
            {
                property.NullValueHandling = NullValueHandling.Ignore;
            }

            property.Ignored = annotations.IgnoreSerialized;
            return property;
        }
    }
}