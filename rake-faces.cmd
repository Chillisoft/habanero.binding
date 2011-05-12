@cd temp/faces 
@rmdir /s /q .svn 
rake --rakefile facesCF-library-rakefile.rb --execute-continue "$faces_version = 'branches/V2.6-CF_Stargate'; $buildscriptpath = 'F:/Systems/HabaneroCommunity/BuildScripts'" 
