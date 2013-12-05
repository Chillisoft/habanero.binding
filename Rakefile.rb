require 'rake'
require 'albacore'

#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

# set up the build script folder so we can pull in shared rake scripts.
# This should be the same for most projects, but if your project is a level
# deeper in the repo you will need to add another ..
bs = File.dirname(__FILE__)
bs = File.join(bs, "/rake-tasks")
$buildscriptpath = File.expand_path(bs)
$:.unshift($buildscriptpath) unless
    $:.include?(bs) || $:.include?($buildscriptpath)

$binaries_baselocation = "bin"
$nuget_baselocation = "nugetArtifacts"
$app_version_hab ='9.9.9.999'
$app_version_bin ='9.9.9.999'
#------------------------build settings--------------------------
require 'rake-settings.rb'

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet,
  #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

#------------------------dependency settings---------------------
#------------------------project settings------------------------
$solution = "source/Habanero.Binding - 2010.sln"
$solutionNuget = '"source/Habanero.Binding - 2010.sln"'
$major_version_hab = ''
$minor_version_hab = ''
$patch_version_hab = ''
$major_version_binding = ''
$minor_version_binding = ''
$patch_version_binding = ''
$nuget_apikey = ''
$nuget_sourceurl = ''
$nuget_publish_version = 'Trunk'
#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default, [:majorhab, :minorhab, :patchhab,:majorbin, :minorbin, :patchbin] => [:updatesubmodules, :setupvars,:build]

desc "Pulls habanero deps from local nuget, builds , tests and pushes faces"
task :build_test_push_internal, [:majorhab, :minorhab, :patchhab,:majorbin, :minorbin, :patchbin, :apikey, :sourceurl] => [:updatesubmodules,:setupvars, :installNugetPackages, :build, :publishHabaneroProgrammaticBindingNugetPackage]

desc "Builds solution, including tests"
task :build, [:majorhab, :minorhab, :patchhab,:majorbin, :minorbin, :patchbin, :apikey, :sourceurl] => [:clean, :setupvars, :set_assembly_version, :installNugetPackages, :msbuild, :copy_to_nuget, :test]

#------------------------Setup Versions---------
desc "Setup Variables"
task :setupvars,:majorhab, :minorhab, :patchhab, :majorbin, :minorbin, :patchbin, :apikey, :sourceurl do |t, args|
	puts cyan("Setup Variables")
	args.with_defaults(:majorhab => "0")
	args.with_defaults(:minorhab => "0")
	args.with_defaults(:patchhab => "0000")
	args.with_defaults(:majorbin => "0")
	args.with_defaults(:minorbin => "0")
	args.with_defaults(:patchbin => "0000")
	args.with_defaults(:apikey => "")
	args.with_defaults(:sourceurl => "")
	$major_version_hab = "#{args[:majorhab]}"
	$minor_version_hab = "#{args[:minorhab]}"
	$patch_version_hab = "#{args[:patchhab]}"
	$major_version_binding = "#{args[:majorbin]}"
	$minor_version_binding = "#{args[:minorbin]}"
	$patch_version_binding = "#{args[:patchbin]}"
	$nuget_apikey = "#{args[:apikey]}"
	$nuget_sourceurl = "#{args[:sourceurl]}"
	$app_version_hab = "#{$major_version_hab}.#{$minor_version_hab}.#{$patch_version_hab}.0"
	$app_version_bin = "#{$major_version_binding}.#{$minor_version_binding}.#{$patch_version_binding}.0"
	puts cyan("Assembly Version Habanero : #{$app_version_hab}")
	puts cyan("Assembly Version Binding : #{$app_version_bin}")
	puts cyan("Nuget key: #{$nuget_apikey} for: #{$nuget_sourceurl}")
end

desc "Restore Nuget Packages"
task :restorepackages do
	puts cyan('lib\nuget.exe restore '+"#{$solutionNuget}")
	system 'lib\nuget.exe restore '+"#{$solutionNuget}"
end

desc "Update Submodules"
task :updatesubmodules do
	puts cyan("Updating Git Submodules")
	system 'git submodule foreach git checkout master'
	system 'git submodule foreach git pull'
end

task :set_assembly_version do
	puts green("Setting Shared AssemblyVersion to: #{$app_version_bin}")
	file_path = "source/Common/AssemblyInfoShared.cs"
	outdata = File.open(file_path).read.gsub(/"9.9.9.999"/, "\"#{$app_version_bin}\"")
	File.open(file_path, 'w') do |out|
		out << outdata
	end	
end
#------------------------build Faces  --------------------

desc "Cleans the bin folder"
task :clean do
	puts cyan("Cleaning bin folder")
	FileUtils.rm_rf 'bin'
	FileUtils.rm_rf $nuget_baselocation	
	FileSystem.ensure_dir_exists $nuget_baselocation
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
    ip.package_names = ["Habanero.Base.#{$nuget_publish_version}",  
						"Habanero.BO.#{$nuget_publish_version}",  
						"Habanero.Console.#{$nuget_publish_version}",  
						"Habanero.DB.#{$nuget_publish_version}", 
						"Habanero.Test.#{$nuget_publish_version}", 
						"Habanero.Smooth.#{$nuget_publish_version}",  
						"Habanero.Naked.#{$nuget_publish_version}",  
						"Habanero.Faces.Base.#{$nuget_publish_version}",  
						"Habanero.Faces.Win.#{$nuget_publish_version}",   
						"Habanero.Faces.Test.Win.#{$nuget_publish_version}",   
						"Habanero.Faces.Test.Base.#{$nuget_publish_version}",  
						"Habanero.Testability.#{$nuget_publish_version}",  
						"Habanero.Testability.Helpers.#{$nuget_publish_version}",  
						"Habanero.Testability.Testers.#{$nuget_publish_version}"]
	ip.SourceUrl = "#{$nuget_sourceurl}/nuget"
	ip.Version = $app_version_hab
end

def copy_nuget_files_to location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.ProgrammaticBinding.dll", location
end

task :copy_to_nuget do
	puts cyan("Copying files to the nuget folder")	
	copy_nuget_files_to $nuget_baselocation
end

desc "Publish the Habanero.ProgrammaticBinding nuget package"
pushnugetpackagesonline :publishHabaneroProgrammaticBindingNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.ProgrammaticBinding.dll"
  package.Nugetid = "Habanero.ProgrammaticBinding.#{$nuget_publish_version}"
  package.Version = $app_version_bin
  package.Description = "Habanero.ProgrammaticBinding"
  package.ApiKey = "#{$nuget_apikey}"
  package.SourceUrl = "#{$nuget_sourceurl}"
end

desc "Builds the solution with msbuild"
msbuild :msbuild do |msb| 
	puts cyan("Building #{$solution} with msbuild")
	msb.update_attributes msbuild_settings
	msb.solution = $solution
end

desc "Runs the tests"
nunit :test do |nunit|
	puts cyan("Running tests")
	nunit.assemblies 'bin\Habanero.Binding.Tests.dll','bin\Habanero.ProgrammaticBinding.Tests.dll','bin\Habanero.ProgrammaticBinding.Tester.Tests.dll'
end