require 'rake'
require 'albacore'


task :default => [:clean_up,:do_habanero,:do_smooth,:do_testability,:do_binding] 

task :do_habanero => [:clean_habanero,:checkout_habanero,:msdo_habanero]

task :do_smooth => [:checkout_smooth,:copy_dlls_to_smooth_lib,:clean_smooth,:msdo_smooth]

task :do_testability => [:checkout_testability,:copy_dlls_to_testability_lib,:clean_testability,:msdo_testability]

task :do_binding => [:copy_dlls_to_binding_lib,:clean_binding,:msdo_binding,:run_nunit]

$binding_sln_path = 'source/Habanero.Binding - 2010.sln'
$Nunit_path = "C:/Program Files (x86)/NUnit 2.5.8/bin/net-2.0/nunit-console-x86.exe"# nunit-console-x86.exe is run to prevent the "Profiler connection not established" error from old Ncover versions
$Nunit_options = '/xml=nunit-result.xml /config=test.config'
    

#do_habanero tasks
task :clean_habanero do
	FileUtils.rm_rf 'temp/Habanero/branches/v2.6/bin/'
end
exec :checkout_habanero do |cmd| 
	cmd.path_to_command = "../../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/Habanero/branches/v2.6" "temp/Habanero/branches/v2.6/" --username chilli --password chilli --non-interactive) 
end

msbuild :msdo_habanero do |msb| #builds habanero with msbuild
    msb.targets :rebuild 
	msb.properties :configuration => :Debug
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
    msb.solution = "temp/Habanero/branches/v2.6/source/Habanero.sln"
  end
  
  
#do_smooth tasks
  task :copy_dlls_to_smooth_lib  do #copies habanero DLLs to smooth lib
	FileUtils.cp Dir.glob('temp/Habanero/branches/v2.6/bin/Habanero*.dll'), 'temp/Smooth1.5/lib'
end

  task :clean_smooth do #deletes bin folder before build
	FileUtils.rm_rf 'temp/Smooth1.5/bin'
end

exec :checkout_smooth do |cmd| #command to check out smooth source using SVN
	cmd.path_to_command = "../../../../Utilities/BuildServer/Subversion/bin/svn.exe"
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/HabaneroCommunity/SmoothHabanero/branches/v1.5/" "temp/Smooth1.5/" --username chilli --password chilli --non-interactive)
end

msbuild :msdo_smooth do |msb| #builds smooth with msbuild
    msb.targets :Build
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
    msb.solution = "temp/Smooth1.5/source/SmoothHabanero_2010.sln"
  end
  
#do_testability tasks
  task :copy_dlls_to_testability_lib  do #copies habanero and smooth DLLs to testability lib
	FileUtils.cp Dir.glob('temp/Habanero/branches/v2.6/bin/Habanero*.dll'), 'temp/Testability1.3_HabV2.6/lib'
	FileUtils.cp Dir.glob('temp/Smooth1.5/bin/Habanero.Smooth*.dll'), 'temp/Testability1.3_HabV2.6/lib'
end
  
    task :clean_testability do #deletes bin folder before build
	FileUtils.rm_rf 'temp/Testability1.3_HabV2.6/bin'
end

exec :checkout_testability do |cmd| #command to check out testability source using SVN
	cmd.path_to_command = "../../../../Utilities/BuildServer/Subversion/bin/svn.exe"
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/HabaneroCommunity/Habanero.Testability/branches/v1.3_HabV2.6/" "temp/Testability1.3_HabV2.6/" --username chilli --password chilli --non-interactive)
end

msbuild :msdo_testability do |msb| #builds testability with msbuild
    msb.targets :Build
	msb.path_to_command = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
	msb.verbosity = "quiet"
  msb.solution = "temp/Testability1.3_HabV2.6/source/Habanero.Testability - 2010.sln"
  end
  
  

task :copy_dlls_to_binding_lib  do #copies habanero, smooth, faces and testability DLLs to faces lib
	FileUtils.cp Dir.glob('temp/Habanero/branches/v2.6/bin/Habanero*.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/Smooth1.5/bin/Habanero.Smooth*.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/Smooth1.5/bin/Habanero.Naked*.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/Testability1.3_HabV2.6/bin/Habanero.Testability*.dll'), 'lib'
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
 nunit.options '/xml=nunit-result.xml'
end

task :clean_up do
FileUtils.rm_rf 'temp'
end