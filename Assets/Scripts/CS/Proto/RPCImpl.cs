using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace ProtoMsg
{
    public class LoginImpl : LoginRpc.LoginRpcBase
    {
        public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            return base.Login(new LoginRequest() { Password = "1", Username = "2" }, context);
        }
    }

    public class LoginSender : LoginRpc.LoginRpcClient
    {
        public override LoginResponse Login(LoginRequest request, Metadata headers = null, DateTime? deadline = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.Login(request, headers, deadline, cancellationToken);
        }

        public override AsyncUnaryCall<LoginResponse> LoginAsync(LoginRequest request, Metadata headers = null,
            DateTime? deadline = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.LoginAsync(request, headers, deadline, cancellationToken);
        }

        public override AsyncUnaryCall<LoginResponse> LoginAsync(LoginRequest request, CallOptions options)
        {
            return base.LoginAsync(request, options);
        }
    }
}