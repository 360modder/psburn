import argparse

ArgumentParser = argparse.ArgumentParser(description="edit psburn csproj to make a release")
ArgumentParser.add_argument("runtime_identifier", help="runtime identifier")
ArgumentParser.add_argument("--csproj", dest="csproj", default="../psburn/psburn.csproj",
                            help="path of psburn csproj file (default: ../psburn/psburn.csproj)")
ArgumentParser.add_argument("-c", "--configuration", dest="configuration", default="Release",
                            help="configuration i.e. Release or Debug (default: Release)")
ArgumentParser.add_argument("--no-self-contained", dest="no_self_contained", action="store_true",
                            help="no self contained netcoreapp3.1 app (default: false)")
ArgumentParser.add_argument("--no-publish-trimmed", dest="no_publish_trimmed", action="store_true",
                            help="no trimmed self contained netcoreapp3.1 app (default: false)")
args = ArgumentParser.parse_args()

new_csproj = []

with open(args.csproj) as f:
    for line in f.readlines():

        if "<RepositoryUrl>htttps://github.com/360modder/psburn</RepositoryUrl>" in line:
            SelfContainedOrNot = "false" if args.no_self_contained else "true"
            TrimmedOrNot = "false" if args.no_publish_trimmed else "true"

            new_csproj.append(line)
            new_csproj.append(f"    <Configuration>{args.configuration}</Configuration>\n")
            new_csproj.append(f"    <SelfContained>{SelfContainedOrNot}</SelfContained>\n")
            new_csproj.append(f"    <PublishTrimmed>{TrimmedOrNot}</PublishTrimmed>\n")
            new_csproj.append(f"    <RuntimeIdentifier>{args.runtime_identifier}</RuntimeIdentifier>\n")
        
        # Skip Tags
        elif "<Configuration>" in line and "</Configuration>" in line:
            continue
        elif "<SelfContained>" in line and "</SelfContained>" in line:
            continue
        elif "<PublishTrimmed>" in line and "</PublishTrimmed>" in line:
            continue
        elif "<RuntimeIdentifier>" in line and "</RuntimeIdentifier>" in line:
            continue

        else:
            new_csproj.append(line)

with open(args.csproj, "w") as f:
    f.writelines(new_csproj)
