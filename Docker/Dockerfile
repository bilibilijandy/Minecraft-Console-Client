# Using the .NET official image for .NET 6.0
FROM mcr.microsoft.com/dotnet/sdk:6.0

# If you want to use a specific build, un-comment the ENV MCC_VERSION line (remove the # from the start)
# Then replace the <version> with a version of your choice (Example: MCC_VERSION=20220817-29)
#ENV MCC_VERSION=<version>

# Force .NET to detect the container so ConsoleWriter uses the FallbackWriter
# (plain Console.WriteLine without ANSI sequences / cursor control), which keeps
# non-PTY (MCSM without pty) output clean: only LF newlines, no stray CR.
ENV DOTNET_RUNNING_IN_CONTAINER=true

# IF you want to use a specific platform then specific the ENV MCC_PLATFORM
# For example to use arm set the environment variable: MCC_PLATFORM=linux-arm64
# `docker run -e MCC_PLATFORM=linux-arm64 -it localhost/minecraft-console-client`

# Copy over the script and give it execution permissions
COPY start-latest.sh /opt/start-latest.sh
RUN chmod +x /opt/start-latest.sh

RUN apt-get update && \ 
    apt-get install -y unzip && \
    mkdir /opt/data

ENTRYPOINT ["/bin/sh", "-c", "/opt/start-latest.sh"]
