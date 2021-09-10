if not exist ..\.git\hooks\pre-push copy build\pre-push ..\.git\hooks\

cd build

"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe" BuildTasks.Dev.targets /fl /flp:logfile=build.log;verbosity=normal

exit
