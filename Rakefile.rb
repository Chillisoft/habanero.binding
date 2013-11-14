require 'rake'
require 'albacore'

#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

# set up the build script folder so we can pull in shared rake scripts.
# This should be the same for most projects, but if your project is a level
# deeper in the repo you will need to add another ..
bs = File.dirname(__FILE__)
bs = File.join(bs, "..") if bs.index("branches") != nil
bs = File.join(bs, "../HabaneroCommunity/BuildScripts")
$buildscriptpath = File.expand_path(bs)
$:.unshift($buildscriptpath) unless
    $:.include?(bs) || $:.include?($buildscriptpath)

if (bs.index("branches") == nil)
	nuget_version = 'Trunk'
	nuget_version_id = '9.9.999'
	
	$nuget_habanero_version	= nuget_version
	$nuget_smooth_version =	nuget_version
	$nuget_testability_version = nuget_version
	$nuget_faces_version = nuget_version
	$nuget_security_version = nuget_version
	
	$nuget_publish_version = nuget_version
	$nuget_publish_version_id = nuget_version_id
else
	$nuget_habanero_version	= 'v2.6-2012-06-12'
	$nuget_smooth_version =	'v1.5_2011-08-24'
	$nuget_testability_version = 'v1.3'
	$nuget_faces_version = 'v2.7-13_02_2012'
	$nuget_security_version = 'v2.7'
	
	$nuget_publish_version = 'v1.2'
	$nuget_publish_version_id = 'v1.2'
end

$binaries_baselocation = "bin"
$nuget_baselocation = "nugetArtifacts"
$app_version ='9.9.9.999'
#------------------------build settings--------------------------
require 'rake-settings.rb'

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet,
  #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

puts cyan("Nuget Details #{$nuget_publish_version }, #{$nuget_publish_version_id} ")	
$major_version = ''
$minor_version = ''
$patch_version = ''
#------------------------dependency settings---------------------
#------------------------project settings------------------------
$solution = "source/Habanero.Binding - 2010.sln"
#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default, [:major, :minor, :patch] => [:build]

desc "Builds solution, including tests"
task :build, [:major, :minor, :patch] => [:clean, :setupversion, :set_assembly_version, :installNugetPackages, :msbuild, :copy_to_nuget, :test, :publishHabaneroProgrammaticBindingNugetPackage]

#------------------------Setup Versions---------
desc "Setup Versions"
task :setupversion,:major ,:minor,:patch do |t, args|
	puts cyan("Setup Versions")
	args.with_defaults(:major => "0")
	args.with_defaults(:minor => "0")
	args.with_defaults(:patch => "0000")
	$major_version = "#{args[:major]}"
	$minor_version = "#{args[:minor]}"
	$patch_version = "#{args[:patch]}"
	$app_version = "#{$major_version}.#{$minor_version}.#{$patch_version}.0"
	puts cyan("Assembly Version #{$app_version}")	
end

task :set_assembly_version do
	puts green("Setting Shared AssemblyVersion to: #{$app_version}")
	file_path = "source/Common/AssemblyInfoShared.cs"
	outdata = File.open(file_path).read.gsub(/"9.9.9.999"/, "\"#{$app_version}\"")
	File.open(file_path, 'w') do |out|
		out << outdata
	end	
end
#------------------------build Faces  --------------------

desc "Cleans the bin folder"
task :clean do
	puts cyan("Cleaning bin folder")
	FileUtils.rm_rf 'bin'
end

svn :update_lib_from_svn do |s|
	s.parameters "update lib"
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
    ip.package_names = ["Habanero.Base.#{$nuget_habanero_version}",  
						"Habanero.BO.#{$nuget_habanero_version}",  
						"Habanero.Console.#{$nuget_habanero_version}",  
						"Habanero.DB.#{$nuget_habanero_version}", 
						"Habanero.Test.#{$nuget_habanero_version}", 
						"Habanero.Smooth.#{$nuget_smooth_version}",  
						"Habanero.Naked.#{$nuget_smooth_version}",  
						"Habanero.Faces.Base.#{$nuget_faces_version}",  
						"Habanero.Faces.Win.#{$nuget_faces_version}",   
						"Habanero.Faces.Test.Win.#{$nuget_faces_version}",   
						"Habanero.Faces.Test.Base.#{$nuget_faces_version}",  
						"Habanero.Testability.#{$nuget_testability_version}",  
						"Habanero.Testability.Helpers.#{$nuget_testability_version}",  
						"Habanero.Testability.Testers.#{$nuget_testability_version}",
						"nunit.Trunk"]
end

def copy_nuget_files_to location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.ProgrammaticBinding.dll", location
end

task :copy_to_nuget do
	puts cyan("Copying files to the nuget folder")	
	copy_nuget_files_to $nuget_baselocation
end

desc "Publish the Habanero.ProgrammaticBinding nuget package"
pushnugetpackages :publishHabaneroProgrammaticBindingNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.ProgrammaticBinding.dll"
  package.Nugetid = "Habanero.ProgrammaticBinding.#{$nuget_publish_version}"
  package.Version = $nuget_publish_version_id
  package.Description = "Habanero.ProgrammaticBinding"
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