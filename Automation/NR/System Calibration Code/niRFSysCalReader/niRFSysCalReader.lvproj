<?xml version='1.0' encoding='UTF-8'?>
<Project Type="Project" LVVersion="17008000">
	<Item Name="My Computer" Type="My Computer">
		<Property Name="server.app.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.control.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.tcp.enabled" Type="Bool">false</Property>
		<Property Name="server.tcp.port" Type="Int">0</Property>
		<Property Name="server.tcp.serviceName" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.tcp.serviceName.default" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.vi.callsEnabled" Type="Bool">true</Property>
		<Property Name="server.vi.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="specify.custom.address" Type="Bool">false</Property>
		<Item Name="niRFSysCalReader.lvlib" Type="Library" URL="../niRFSysCalReader.lvlib"/>
		<Item Name="Pathloss XML Reader.lvlib" Type="Library" URL="../../Pathloss XML Reader/Pathloss XML Reader.lvlib"/>
		<Item Name="Dependencies" Type="Dependencies">
			<Item Name="vi.lib" Type="Folder">
				<Item Name="Clear Errors.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Clear Errors.vi"/>
				<Item Name="Error Cluster From Error Code.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Error Cluster From Error Code.vi"/>
				<Item Name="VariantType.lvlib" Type="Library" URL="/&lt;vilib&gt;/Utility/VariantDataType/VariantType.lvlib"/>
			</Item>
		</Item>
		<Item Name="Build Specifications" Type="Build">
			<Item Name="NI RF System Calibration Reader" Type="Source Distribution">
				<Property Name="Bld_buildCacheID" Type="Str">{BA36E517-EA00-483C-93AD-94B4B2AF7CBB}</Property>
				<Property Name="Bld_buildSpecName" Type="Str">NI RF System Calibration Reader</Property>
				<Property Name="Bld_excludedDirectory[0]" Type="Path">vi.lib</Property>
				<Property Name="Bld_excludedDirectory[0].pathType" Type="Str">relativeToAppDir</Property>
				<Property Name="Bld_excludedDirectory[1]" Type="Path">resource/objmgr</Property>
				<Property Name="Bld_excludedDirectory[1].pathType" Type="Str">relativeToAppDir</Property>
				<Property Name="Bld_excludedDirectory[2]" Type="Path">/C/ProgramData/National Instruments/InstCache/15.0</Property>
				<Property Name="Bld_excludedDirectory[3]" Type="Path">instr.lib</Property>
				<Property Name="Bld_excludedDirectory[3].pathType" Type="Str">relativeToAppDir</Property>
				<Property Name="Bld_excludedDirectory[4]" Type="Path">user.lib</Property>
				<Property Name="Bld_excludedDirectory[4].pathType" Type="Str">relativeToAppDir</Property>
				<Property Name="Bld_excludedDirectoryCount" Type="Int">5</Property>
				<Property Name="Bld_localDestDir" Type="Path">/C/P4/Sales/projects/RefApps/RF System Calibration/dev/3.1/tunde/Builds/NI RF System Calibration Reader/Temp/user.lib</Property>
				<Property Name="Bld_previewCacheID" Type="Str">{F2BCF194-8177-48AF-A2DA-76B896E38916}</Property>
				<Property Name="Bld_version.major" Type="Int">1</Property>
				<Property Name="Destination[0].destName" Type="Str">Destination Directory</Property>
				<Property Name="Destination[0].path" Type="Path">/C/P4/Sales/projects/RefApps/RF System Calibration/dev/3.1/tunde/Builds/NI RF System Calibration Reader/Temp/user.lib</Property>
				<Property Name="Destination[0].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="Destination[1].destName" Type="Str">Support Directory</Property>
				<Property Name="Destination[1].path" Type="Path">/C/P4/Sales/projects/RefApps/RF System Calibration/dev/3.1/tunde/Builds/NI RF System Calibration Reader/Temp/user.lib/_subVIs</Property>
				<Property Name="Destination[1].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="Destination[2].destName" Type="Str">examples</Property>
				<Property Name="Destination[2].path" Type="Path">/C/P4/Sales/projects/RefApps/RF System Calibration/dev/3.1/tunde/Builds/NI RF System Calibration Reader/Temp/examples</Property>
				<Property Name="Destination[2].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="Destination[3].destName" Type="Str">_INCLUDES</Property>
				<Property Name="Destination[3].path" Type="Path">/C/P4/Sales/projects/RefApps/RF System Calibration/dev/3.1/tunde/Builds/NI RF System Calibration Reader/Temp/user.lib/_INCLUDES</Property>
				<Property Name="Destination[3].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="Destination[4].destName" Type="Str">_subVIs</Property>
				<Property Name="Destination[4].path" Type="Path">/C/P4/Sales/projects/RefApps/RF System Calibration/dev/3.1/tunde/Builds/NI RF System Calibration Reader/Temp/user.lib/_subVIs</Property>
				<Property Name="Destination[4].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="Destination[5].destName" Type="Str">API</Property>
				<Property Name="Destination[5].path" Type="Path">/C/P4/Sales/projects/RefApps/RF System Calibration/dev/3.1/tunde/Builds/NI RF System Calibration Reader/Temp/user.lib/NI RF System Calibration Reader API</Property>
				<Property Name="Destination[5].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="Destination[6].destName" Type="Str">Pathloss XML Reader</Property>
				<Property Name="Destination[6].path" Type="Path">/C/P4/Sales/projects/RefApps/RF System Calibration/dev/3.1/tunde/Builds/NI RF System Calibration Reader/Temp/user.lib/_Dependencies/Pathloss XML Reader</Property>
				<Property Name="Destination[6].path.type" Type="Str">&lt;none&gt;</Property>
				<Property Name="Destination[6].preserveHierarchy" Type="Bool">true</Property>
				<Property Name="DestinationCount" Type="Int">7</Property>
				<Property Name="Source[0].itemID" Type="Str">{A2ABF5AA-C533-432B-A1A1-61312929CEC3}</Property>
				<Property Name="Source[0].type" Type="Str">Container</Property>
				<Property Name="Source[1].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[1].itemID" Type="Ref">/My Computer/niRFSysCalReader.lvlib</Property>
				<Property Name="Source[1].Library.allowMissingMembers" Type="Bool">true</Property>
				<Property Name="Source[1].sourceInclusion" Type="Str">Include</Property>
				<Property Name="Source[1].type" Type="Str">Library</Property>
				<Property Name="Source[2].Container.applyDestination" Type="Bool">true</Property>
				<Property Name="Source[2].Container.depDestIndex" Type="Int">0</Property>
				<Property Name="Source[2].destinationIndex" Type="Int">5</Property>
				<Property Name="Source[2].itemID" Type="Ref">/My Computer/niRFSysCalReader.lvlib/API</Property>
				<Property Name="Source[2].type" Type="Str">Container</Property>
				<Property Name="Source[3].Container.applyDestination" Type="Bool">true</Property>
				<Property Name="Source[3].Container.depDestIndex" Type="Int">0</Property>
				<Property Name="Source[3].destinationIndex" Type="Int">2</Property>
				<Property Name="Source[3].itemID" Type="Ref">/My Computer/niRFSysCalReader.lvlib/Example</Property>
				<Property Name="Source[3].type" Type="Str">Container</Property>
				<Property Name="Source[4].Container.applyDestination" Type="Bool">true</Property>
				<Property Name="Source[4].Container.depDestIndex" Type="Int">0</Property>
				<Property Name="Source[4].destinationIndex" Type="Int">3</Property>
				<Property Name="Source[4].itemID" Type="Ref">/My Computer/niRFSysCalReader.lvlib/INCLUDES</Property>
				<Property Name="Source[4].type" Type="Str">Container</Property>
				<Property Name="Source[5].Container.applyDestination" Type="Bool">true</Property>
				<Property Name="Source[5].Container.depDestIndex" Type="Int">0</Property>
				<Property Name="Source[5].destinationIndex" Type="Int">4</Property>
				<Property Name="Source[5].itemID" Type="Ref">/My Computer/niRFSysCalReader.lvlib/subVIs</Property>
				<Property Name="Source[5].type" Type="Str">Container</Property>
				<Property Name="Source[6].destinationIndex" Type="Int">6</Property>
				<Property Name="Source[6].itemID" Type="Ref">/My Computer/Pathloss XML Reader.lvlib</Property>
				<Property Name="Source[6].Library.allowMissingMembers" Type="Bool">true</Property>
				<Property Name="Source[6].sourceInclusion" Type="Str">Include</Property>
				<Property Name="Source[6].type" Type="Str">Library</Property>
				<Property Name="Source[7].Container.applyDestination" Type="Bool">true</Property>
				<Property Name="Source[7].Container.depDestIndex" Type="Int">0</Property>
				<Property Name="Source[7].destinationIndex" Type="Int">6</Property>
				<Property Name="Source[7].itemID" Type="Ref">/My Computer/Pathloss XML Reader.lvlib/API</Property>
				<Property Name="Source[7].type" Type="Str">Container</Property>
				<Property Name="Source[8].Container.applyDestination" Type="Bool">true</Property>
				<Property Name="Source[8].Container.depDestIndex" Type="Int">0</Property>
				<Property Name="Source[8].destinationIndex" Type="Int">6</Property>
				<Property Name="Source[8].itemID" Type="Ref">/My Computer/Pathloss XML Reader.lvlib/Controls</Property>
				<Property Name="Source[8].type" Type="Str">Container</Property>
				<Property Name="Source[9].Container.applyDestination" Type="Bool">true</Property>
				<Property Name="Source[9].Container.depDestIndex" Type="Int">0</Property>
				<Property Name="Source[9].destinationIndex" Type="Int">6</Property>
				<Property Name="Source[9].itemID" Type="Ref">/My Computer/Pathloss XML Reader.lvlib/Example</Property>
				<Property Name="Source[9].type" Type="Str">Container</Property>
				<Property Name="SourceCount" Type="Int">10</Property>
			</Item>
		</Item>
	</Item>
</Project>
