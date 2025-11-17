set -a
source .env
set +a

dotnet tool install -g dotnet-ef && dotnet ef dbcontext scaffold "$CONN_STR" Npgsql.EntityFrameworkCore.PostgreSQL   --context DbContext     --no-onconfiguring        --schema library   --force