require 'rake'
require 'albacore'


task :default => [:clean_up,:do_habanero,:do_smooth,:do_testability,:do_binding] 

task :do_habanero => [:clean_habanero,:checkout_habanero,:msdo_habanero]

task :do_smooth => [:checkout_smooth,:copy_dlls_to_smooth_lib,:clean_smooth,:msdo_smooth]

task :do_testability => [:checkout_testability,:copy_dlls_to_testability_lib,:clean_testability,:msdo_testability]

task :do_binding => [:copy_dlls_to_binding_lib,:clean_binding,:msdo_binding,:run_nunit,:commit_lib]

$binding_sln_path = 'source/Habanero.Binding - 2010.sln'
$Nunit_path = "C:/Program Files (x86)/NUnit 2.5.8/bin/net-2.0/nunit-console-x86.exe"# nunit-console-x86.exe is run to prevent the "Profiler connection not established" error from old Ncover versions
$Nunit_options = '/xml=nunit-result.xml'
    


task :clean_habanero do
	FileUtils.rm_rf 'temp/Habanero/trunk/bin/'
end
exec :checkout_habanero do |cmd| 
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/Habanero/trunk" "temp/Habanero/trunk/" --username chilli --password chilli --non-interactive) 
end

msbuild :msdo_habanero do |msb| #builds habanero with msbuild
    msb.targets :rebuild 
	msb.properties :configuration => :Debug
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
    msb.solution = "temp/Habanero/trunk/source/Habanero.sln"
  end
  
  
#do_smooth tasks
  task :copy_dlls_to_smooth_lib  do #copies habanero DLLs to smooth lib
	FileUtils.cp Dir.glob('temp/Habanero/trunk/bin/Habanero*.dll'), 'temp/HabaneroCommunity/SmoothHabanero/trunk/lib'
end

  task :clean_smooth do #deletes bin folder before build
	FileUtils.rm_rf 'temp/HabaneroCommunity/SmoothHabanero/trunk/bin'
end

exec :checkout_smooth do |cmd| #command to check out smooth source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe"
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/HabaneroCommunity/SmoothHabanero/trunk/" "temp/HabaneroCommunity/SmoothHabanero/trunk/" --username chilli --password chilli --non-interactive)
end

msbuild :msdo_smooth do |msb| #builds smooth with msbuild
    msb.targets :Build
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
    msb.solution = "temp/HabaneroCommunity/SmoothHabanero/trunk/source/SmoothHabanero_2010.sln"
  end
  
#do_testability tasks
  task :copy_dlls_to_testability_lib  do #copies habanero and smooth DLLs to testability lib
	FileUtils.cp Dir.glob('temp/Habanero/trunk/bin/Habanero*.dll'), 'temp/HabaneroCommunity/Habanero.Testability/Trunk/lib'
	FileUtils.cp Dir.glob('temp/HabaneroCommunity/SmoothHabanero/trunk/bin/Habanero.Smooth*.dll'), 'temp/HabaneroCommunity/Habanero.Testability/Trunk/lib'
end
  
    task :clean_testability do #deletes bin folder before build
	FileUtils.rm_rf 'temp/HabaneroCommunity/Habanero.Testability/Trunk/bin'
end

exec :checkout_testability do |cmd| #command to check out testability source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe"
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/HabaneroCommunity/Habanero.Testability/Trunk/" "temp/HabaneroCommunity/Habanero.Testability/Trunk/" --username chilli --password chilli --non-interactive)
end

msbuild :msdo_testability do |msb| #builds testability with msbuild
    msb.targets :Build
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
  msb.solution = "temp/HabaneroCommunity/Habanero.Testability/Trunk/source/Habanero.Testability - 2010.sln"
  end
  
  

task :copy_dlls_to_binding_lib  do #copies habanero, smooth, faces and testability DLLs to faces lib
	FileUtils.cp Dir.glob('temp/Habanero/trunk/bin/Habanero*.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/HabaneroCommunity/SmoothHabanero/trunk/bin/Habanero.Smooth*.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/HabaneroCommunity/SmoothHabanero/trunk/bin/Habanero.Naked*.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/HabaneroCommunity/Habanero.Testability/Trunk/bin/Habanero.Testability*.dll'), 'lib'
end 

task :clean_binding do #deletes bin folder before build
	FileUtils.rm_rf 'bin'
end

msbuild :msdo_binding do |msb| #builds faces with msbuild
    msb.targets :Build
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
  msb.solution = $binding_sln_path
  end
  
nunit :run_nunit do |nunit|
 nunit.path_to_command = $Nunit_path
 nunit.assemblies 'bin\Habanero.Binding.Tests.dll'
 nunit.options $Nunit_options
end

task :clean_up do
FileUtils.rm_rf 'temp'
end

exec :commit_lib do |cmd| #command to check out habanero source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(ci -m autocheckin --username chilli --password chilli) 
end