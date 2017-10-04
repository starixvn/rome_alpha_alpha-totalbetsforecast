if not exist ..\.git\hooks\pre-push copy build\pre-push ..\.git\hooks\

cd build

"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" BuildTasks.Dev.targets /fl /flp:logfile=build.log;verbosity=normal

exit
