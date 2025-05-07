using Autofac;

namespace Tr1ppy.Cryptography.Password.Integration.Autofac;

public static class PasswordCryptographyContainerBuilderExtensions 
{
    public static void AddPasswordCryptography
    (
       this ContainerBuilder builder,
       Action<PasswordCryptographyServiceSettings> configure
    )
    {
        builder.RegisterInstance(new PasswordCryptographyServiceSettings())
               .OnActivated(args => configure(args.Instance));

        builder.RegisterType<PasswordCryptographyService>()
               .AsSelf()
               .SingleInstance();
    }
}
