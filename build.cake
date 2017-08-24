#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.1
#tool nuget:?package=NUnit.Extension.TeamCityEventListener
#tool "nuget:?package=gitlink"
#addin nuget:?package=Cake.DoInDirectory

var target = Argument("target", "Default");
var configuration = "Release";
var output = Directory("build");
var outputNuGet = output + Directory("nuget");
var solutions = GetFiles("./**/*.sln");

Task("Default")
    //.IsDependentOn("Package");
    .IsDependentOn("GitLink");

Task("Clean")
    .Does(() =>
    {
        foreach(var solution in solutions)
        {
            DotNetCoreClean(solution.GetDirectory().FullPath, new DotNetCoreCleanSettings{Configuration = "Debug"});
            DotNetCoreClean(solution.GetDirectory().FullPath, new DotNetCoreCleanSettings{Configuration = "Release"});
        }
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        foreach(var solution in solutions)
        {
            DotNetCoreBuild(solution.GetDirectory().FullPath, new DotNetCoreBuildSettings {
                Configuration = configuration,
            });
        }
    });

Task("GitLink")
    .IsDependentOn("Build")
    .Does(() =>
    {
//        Information(baseDir);
        var pdbsInSolution = GetFiles(Context.Environment.WorkingDirectory + "/src/**/*.pdb");
        //foreach(var solution in solutions)
        {
            //Information(solution.GetDirectory().FullPath)
            //GitLink(solution.GetDirectory().FullPath + "/", new GitLinkSettings {});

            //var pdbsInSolution = GetFiles(solution.GetDirectory().FullPath + "/**/*.pdb");

            foreach (var pdb in pdbsInSolution.Where(f => (DateTime.UtcNow.Subtract(new FileInfo(f.FullPath).LastWriteTimeUtc).TotalMinutes < 5)))
            //foreach (var pdb in pdbsInSolution)
            {
                Information(DateTime.UtcNow.Subtract(new FileInfo(pdb.FullPath).LastWriteTimeUtc).TotalMinutes);
                GitLink(pdb.FullPath, new GitLinkSettings {});
                // While waiting for https://github.com/cake-build/cake/issues/1734
                //var gitLink = Context.Tools.Resolve("gitlink.exe");
                //Information(gitLink);

                 //StartProcess(gitLink, new ProcessSettings { Arguments = pdb.FullPath + " --baseDir .."});
            }
        }
    });


Task("Package")
    .IsDependentOn("GitLink")
    .Does(() =>
    {
        foreach(var solution in solutions)
        {
            DotNetCorePack(solution.GetDirectory().FullPath, new DotNetCorePackSettings {
                Configuration = configuration,
                OutputDirectory = outputNuGet,
                IncludeSymbols = true,
                NoBuild = true
            });
        }
});

RunTarget(target);