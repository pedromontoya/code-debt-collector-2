using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Neudesic.Tools.CodeDebt.Configuration
{
	public abstract class SettingsCollectionBase<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
	{
		public string PropertyName { get; private set; }
		public SettingsCollectionBase(string propertyName)
		{
			this.PropertyName = propertyName;
		}
		protected override string ElementName
		{
			get
			{
				return PropertyName;
			}
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMapAlternate;
			}
		}

		protected override bool IsElementName(string elementName)
		{
			return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new T();
		}
	}
}
