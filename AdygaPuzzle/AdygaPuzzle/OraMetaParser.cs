
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace System
{
   public class SerializableAttribute : Attribute
   {
       //this does nothing
   }  
}

namespace AdygaPuzzle
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class image
    {

        private imageLayer[] stackField;

        private ushort hField;

        private ushort wField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("layer", IsNullable = false)]
        public imageLayer[] stack
        {
            get
            {
                return this.stackField;
            }
            set
            {
                this.stackField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort h
        {
            get
            {
                return this.hField;
            }
            set
            {
                this.hField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort w
        {
            get
            {
                return this.wField;
            }
            set
            {
                this.wField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class imageLayer
    {

        private string compositeopField;

        private string nameField;

        private decimal opacityField;

        private string srcField;

        private string visibilityField;

        private ushort xField;

        private ushort yField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("composite-op")]
        public string compositeop
        {
            get
            {
                return this.compositeopField;
            }
            set
            {
                this.compositeopField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal opacity
        {
            get
            {
                return this.opacityField;
            }
            set
            {
                this.opacityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string src
        {
            get
            {
                return this.srcField;
            }
            set
            {
                this.srcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string visibility
        {
            get
            {
                return this.visibilityField;
            }
            set
            {
                this.visibilityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort x
        {
            get
            {
                return this.xField;
            }
            set
            {
                this.xField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort y
        {
            get
            {
                return this.yField;
            }
            set
            {
                this.yField = value;
            }
        }
    }

    internal static class ParseHelpers
    {
        public static T ParseXML<T>(Stream input) where T : class
        {
            var reader = XmlReader.Create(input, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }
    }

}
