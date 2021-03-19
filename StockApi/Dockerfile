FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /code

COPY . .
RUN dotnet publish -c Release -o /output

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runtime
WORKDIR /cmd

COPY --from=build /output .
CMD [ "dotnet", "StockApi.dll" ]