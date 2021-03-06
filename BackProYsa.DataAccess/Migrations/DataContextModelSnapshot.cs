// <auto-generated />
using BackProYsa.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BackProYsa.DataAccess.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BackProYsa.DataAccess.Domain.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("BackProYsa.DataAccess.Domain.NeuralBpLayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("HiddenLayerListStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HiddenNum")
                        .HasColumnType("int");

                    b.Property<string>("HiddenUnitCount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InputLayerListStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InputNum")
                        .HasColumnType("int");

                    b.Property<string>("InputUnitCount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LayerType")
                        .HasColumnType("int");

                    b.Property<double>("LearningRate")
                        .HasColumnType("float");

                    b.Property<string>("MaxErrorCount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumOfPatterns")
                        .HasColumnType("int");

                    b.Property<string>("OutputCount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OutputLayerListStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OutputNum")
                        .HasColumnType("int");

                    b.Property<string>("PreInputLayerListStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PreInputNum")
                        .HasColumnType("int");

                    b.Property<int>("av_ImageHeight")
                        .HasColumnType("int");

                    b.Property<int>("av_ImageWidth")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("NeuralBpLayer");
                });

            modelBuilder.Entity("BackProYsa.DataAccess.Domain.NeuralNodes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NeuralNodes");
                });
#pragma warning restore 612, 618
        }
    }
}
