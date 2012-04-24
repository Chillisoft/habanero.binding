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
$habanero_version = 'branches/v2.6-DotNet2CompactFramework'
require 'rake-habanero.rb'

$faces_version = 'branches/V2.6-CF_Stargate'
require 'rake-facesCF.rb'

$smooth_version = 'branches/v1.5_CF_Stargate'
require 'rake-smoothCF.rb'

$testability_version = 'branches/v2.5-CF'
require 'rake-testabilityCF.rb'

#------------------------project settings------------------------
$basepath = 'http://delicious:8080/svn/habanero/HabaneroCommunity/Habanero.Binding/branches/v1.2_ForCF_Stargate'
$solution = "source/Habanero.Binding - 2008_CF.sln"

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default => [:build_all]

desc "Rake Faces, smooth and testability"
task :rake_dependencies => [:rake_habanero, :rake_smooth, :rake_faces, :rake_testability]

desc "Rakes dependencies, builds solution"
task :build_all => [:create_temp, :rake_dependencies, :build, :delete_temp]

desc "Rakes dependencies, updates lib only"
task :rake_and_update_lib => [:create_temp, :rake_dependencies, :updatelib, :delete_temp]

desc "Builds solution, including tests"
task :build => [:clean, :updatelib, :msbuild, :test]

desc "Pushes Habanero into the local nuget folder"
task :nuget => [:publishProgrammaticBindingNugetPackage]

#------------------------build Faces  --------------------

desc "Cleans the bin folder"
task :clean do
	puts cyan("Cleaning bin folder")
	FileUtils.rm_rf 'bin'
end

svn :update_lib_from_svn do |s|
	s.parameters "update lib"
end

task :updatelib => :update_lib_from_svn do 
	puts cyan("Updating lib")
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.xml'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.xml'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Console.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Console.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Console.xml'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.DB.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.DB.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.DB.xml'), 'lib'
	
	FileUtils.cp Dir.glob('temp/bin/Habanero.Smooth.dll'), 'lib'	
	FileUtils.cp Dir.glob('temp/bin/Habanero.Smooth.pdb'), 'lib'	
	
	FileUtils.cp Dir.glob('temp/bin/Habanero.Faces.Base.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Faces.Base.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Faces.Base.xml'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Faces.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Faces.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Faces.xml'), 'lib'
	
	FileUtils.cp Dir.glob('temp/bin/Habanero.Testability.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Testability.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Testability.xml'), 'lib'	
	FileUtils.cp Dir.glob('temp/bin/Habanero.Testability.Helpers.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Testability.Helpers.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Testability.Helpers.xml'), 'lib'	
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