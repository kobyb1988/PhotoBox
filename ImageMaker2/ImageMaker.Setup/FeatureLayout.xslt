<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    exclude-result-prefixes="wix"
    >

  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="wix:Wix">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:for-each select="wix:Fragment/wix:DirectoryRef/wix:Directory">
        <Fragment xmlns="http://schemas.microsoft.com/wix/2006/wi">
          <ComponentGroup Id="{@Name}">
            <xsl:for-each select="descendant::wix:Component">
              <ComponentRef Id="{@Id}"/>
            </xsl:for-each>
          </ComponentGroup>
        </Fragment>
      </xsl:for-each>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>