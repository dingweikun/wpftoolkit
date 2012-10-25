﻿/************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2010-2012 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   This program can be provided to you by Xceed Software Inc. under a
   proprietary commercial license agreement for use in non-Open Source
   projects. The commercial version of Extended WPF Toolkit also includes
   priority technical support, commercial updates, and many additional 
   useful WPF controls if you license Xceed Business Suite for WPF.

   Visit http://xceed.com and follow @datagrid on Twitter.

  **********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.ComponentModel;

namespace Xceed.Utils.XmlSerialization
{
  /// <summary>
  /// This class can serialize or desirialize the specified type, 
  /// or any derived type, using an XmlSerializer.
  /// 
  /// To use this class, simply put an XmlElement attribute on the 
  /// property to serialize and pass the type of this class to
  /// the Type parameter of the XmlElement. This can also be used 
  /// with the XmlArrayItem attribute.
  /// </summary>
  /// <typeparam name="T">The base type for the object to serialize or deserialize.</typeparam>
  public class XmlSerializableBaseType<T> : IXmlSerializable
  {
    #region Implicit Operators

    public static implicit operator XmlSerializableBaseType<T>( T item )
    {
      return ( item == null )
        ? null
        : new XmlSerializableBaseType<T>( item );
    }

    public static implicit operator T( XmlSerializableBaseType<T> item )
    {
      return ( item.Equals( default( T ) ) )
        ? default( T )
        : item.m_item;
    }

    #endregion Implicit Operators

    #region CONSTRUCTORS

    public XmlSerializableBaseType()
    {
    }

    public XmlSerializableBaseType( T item )
    {
      m_item = item;
    }

    #endregion CONSTRUCTORS

    #region IXmlSerializable Members

    XmlSchema IXmlSerializable.GetSchema()
    {
      return null;
    }

    void IXmlSerializable.ReadXml( XmlReader reader )
    {
      Type type = XmlSerializableBaseType<T>.GetType( reader.GetAttribute( "type" ) );

      reader.ReadStartElement();
      m_item = ( T )new XmlSerializer( type ).Deserialize( reader );
      reader.ReadEndElement();
    }

    void IXmlSerializable.WriteXml( XmlWriter writer )
    {
      Type type = m_item.GetType();

      writer.WriteAttributeString( 
        "type", string.Format( "{0}, {1}", type.FullName, type.Assembly.GetName().Name ) );

      new XmlSerializer( type ).Serialize( writer, m_item );
    }

    #endregion IXmlSerializable Members

    #region PRIVATE METHODS

    private static Type GetType( string typeName )
    {
      if( mg_types.ContainsKey( typeName ) )
        return mg_types[ typeName ];

      Type type = null;
      int commaPosition = typeName.IndexOf( ',' );

      if( commaPosition > 0 )
      {
        type = Type.GetType( typeName.Substring( 0, commaPosition ) );
      }

      if( type == null )
      {
        type = Type.GetType( typeName );
      }

      if( type != null )
        mg_types.Add( typeName, type );

      return type;
    }

    #endregion PRIVATE METHODS

    #region PRIVATE FIELDS

    private static Dictionary<string, Type> mg_types = new Dictionary<string, Type>();
    private T m_item;

    #endregion PRIVATE FIELDS
  }
}
