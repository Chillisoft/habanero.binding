#rake binding
require 'rake'
require 'albacore'

#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

# set up the build script folder so we can pull in shared rake scripts.
# This should be the same for most projects, but if your project is a level
# deeper in the repo you will need to add another ..
bs = File.dirname(__FILE__)
bs = File.join(bs, "..") if bs.index("branches") != nil
bs = File.join(bs, "../../../HabaneroCommunity/BuildScripts")
$buildscriptpath = File.expand_path(bs)
$:.unshift($buildscriptpath) unless
    $:.include?(bs) || $:.include?($buildscriptpath)

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
$basepath = 'http://delicious:8080/svn/habanero/HabaneroCommunity/Habanero.Binding/branches/v1.2_ForCF_Stargate'
$solution = "source/Habanero.Binding - 2008_CF.sln"

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default => [:build_all]

desc "Rakes dependencies, builds solution"
task :build_all => [:create_temp, :build, :delete_temp, :nuget]

desc "Builds solution, including tests"
task :build => [:clean, :installNugetPackages, :msbuild, :test]

desc "Pushes Habanero into the local nuget folder"
task :nuget => [:publishProgrammaticBindingNugetPackage]

#------------------------build Faces  --------------------

desc "Cleans the bin folder"
task :clean do
	puts cyan("Cleaning bin folder")
	FileUtils.rm_rf 'bin'
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
   ip.package_names = ["Habanero.Base.v2.6-CF_Stargate", 
						"Habanero.BO.v2.6-CF_Stargate",  
						"Habanero.Faces.Base.V2.6-CF_Stargate",
						"Habanero.Faces.V2.6-CF_Stargate",
						"Habanero.Smooth.v1.5_CF_Stargate", 
						"Habanero.Testability.v2.5-CF_Stargate",  
						"Habanero.Testability.Helpers.v2.5-CF_Stargate"]
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
	nunit.assemblies 'bin\Habanero.ProgrammaticBinding.Tests.dll'
end

svn :commitlib do |s|
	puts cyan("Commiting lib")
	s.parameters "ci lib -m autocheckin"
end

desc "Publish the Habanero.ProgrammaticBinding nuget package"
pushnugetpackages :publishProgrammaticBindingNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.ProgrammaticBinding.dll"
  package.Nugetid = "Habanero.ProgrammaticBinding.v1.2_ForCF_Stargate"
  package.Version = "1.2"
  package.Description = "Habanero.ProgrammaticBinding"
end