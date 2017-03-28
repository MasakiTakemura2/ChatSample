#!/usr/bin/env ruby
#
# PostprocessBuildPlayer
#   Tested on Ruby 1.8.7, Gem 1.3.6, and xcodeproj 0.3.0
#   Created by akisute (http://akisute.com)
#   Licensed under The MIT License: http://opensource.org/licenses/mit-license.php
#
require 'rubygems'
gem "xcodeproj", "=0.3.0"
require 'xcodeproj'
require 'pathname'
 
#
# Define utility functions
#
def add_system_frameworks_to_project(proj, framework_names, option=:required)
    proj.targets.each do |target|
        if target.name == "Unity-iPhone-simulator" then
            next
        end 
        framework_names.each { |framework_name|
            framework = proj.add_system_framework(framework_name)
            ref = Xcodeproj::Project::PBXBuildFile.new(proj, nil, { 'fileRef' => framework.uuid})
            if option == :optional then
                ref.settings = {"ATTRIBUTES" => ["Weak"] }
            end
            if target.respond_to?(:build_phases) then
                # xcodeproj 0.3.0 or greater
                phase = target.build_phases.find { |phase| phase.is_a?(Xcodeproj::Project::PBXFrameworksBuildPhase) }
                phase.build_files << ref
            else
                # xcodeproj 0.1.0
                phase = target.buildPhases.find { |phase| phase.is_a?(Xcodeproj::Project::PBXFrameworksBuildPhase) }
                phase.files << ref
            end
            puts "Added system framework: " + framework_name + " as " + option.id2name
        }
    end
end
 
#
# Define build directory path
# -> Will be suppried as argv if run by Unity
# -> Else, assume UNITY_PROJECT_ROOT/build is a build directory
#
buildpath = (ARGV[0]) ? ARGV[0] : File.expand_path(File.dirname($0)) + "/../../build"
puts "PostprocessBuildPlayer running on build directory: " + buildpath
 
#
# Add System frameworks required to build
#
projpath = buildpath + "/Unity-iPhone.xcodeproj"
proj = Xcodeproj::Project.new(projpath)
add_system_frameworks_to_project(proj, ["StoreKit", "Security", "MessageUI", "" ], :required)
add_system_frameworks_to_project(proj, ["Twitter", "Social", "CoreData", "AdSupport", "iAd"], :optional)
proj.save_as(projpath)    