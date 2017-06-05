<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
  <html>
  <body>
  <h1>Tests</h1>
    <xsl:for-each select="catalog/test">
	<h2><xsl:value-of select="title"/></h2>
	<table border="1">
    <tr bgcolor="#9acd32">
      <th>ID</th>
      <th>Topic</th>
    </tr>
    <tr>
      <td><xsl:value-of select="id"/></td>
      <td><xsl:value-of select="topic"/></td>
    </tr>
    </table>
    
    <xsl:for-each select="tests">
    <h3>Test</h3>
    <ul>
      <li><xsl:value-of select="test"/></li>
    </ul>
    </xsl:for-each>
    
    <xsl:for-each select="expectations">
    <h3>Expectation <xsl:value-of select="@id"/></h3>
    <!--
      Show additional comment if present in XML.
      http://stackoverflow.com/questions/825831/check-if-a-string-is-null-or-empty-in-xslt
    -->
    <xsl:if test="@comment">
      <p><xsl:value-of select="@comment"/></p>
    </xsl:if>

    <ul>
      <xsl:for-each select="expectation">
        <li><xsl:value-of select="."/></li>
        <!-- List known bugs if any listed -->
	    <xsl:if test="bug">
        <table border="0" cellspacing="2">
        <tr>
          <xsl:for-each select="./bug">
          <td><b><font color="#9a0000">Known Bug:</font></b></td><td><xsl:value-of select="."/></td>
          </xsl:for-each>
        </tr>
        </table>
	    </xsl:if>
      </xsl:for-each>
    </ul>
    </xsl:for-each>
    
    <pre/>
    <hr/>
    </xsl:for-each>
  </body>
  </html>
</xsl:template>

</xsl:stylesheet> 