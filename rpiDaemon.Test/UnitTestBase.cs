using System;
using AutoFixture;
using NSubstitute;
using NUnit.Framework;
using SimpleInjector;

namespace rpiDaemon.Test
{
    public abstract class UnitTestBase
    {
        protected Container Container { get; private set; }
        protected  Fixture Fixture = new Fixture();
        protected Lifestyle DefaultLifestyle { get; set; } = Lifestyle.Singleton;

        [SetUp]
        public virtual void SetUp()
        {
            Container = new Container();

            Container.ResolveUnregisteredType += (sender, e) =>
            {
                try
                {
                    var substitute = Substitute.For(new[] { e.UnregisteredServiceType }, null);
                    Console.WriteLine($"Subbed {e.UnregisteredServiceType}");
                    if (e.UnregisteredServiceType.IsInterface ||
                        !e.UnregisteredServiceType.IsClass && !e.UnregisteredServiceType.IsAbstract)
                        e.Register(DefaultLifestyle.CreateRegistration(e.UnregisteredServiceType, () => substitute,
                            Container));
                }
                catch (Exception)
                {
                    // Swallow.
                }
            };
            RegisterDependencies();
        }

        public void RegisterDependencies()
        {
            // Container.RegisterSingleton<IRepositories, Repositories>();
        }


        public T Get<T>() where T : class
        {
            try
            {
                return Container.GetInstance<T>();
            }
            catch (Exception e) when (!(e is DiagnosticVerificationException ||
                                        e.InnerException is DiagnosticVerificationException))
            {
                return Substitute.For<T>();
            }
        }
    }

    public abstract class UnitTestBase<T> : UnitTestBase where T : class
    {
        // Lazy to allow additional registrations before accessing TestSubject
        private Lazy<T> _testSubjectLazy;

        protected T TestSubject => _testSubjectLazy.Value;

        [SetUp]
        public new virtual void SetUp()
        {
            Container.Register<T>(DefaultLifestyle);
            Container.Options.ResolveUnregisteredConcreteTypes = true;

            _testSubjectLazy = new Lazy<T>(() => Container.GetInstance<T>());
        }
    }
}