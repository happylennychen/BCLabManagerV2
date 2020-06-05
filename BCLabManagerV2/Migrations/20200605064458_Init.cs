using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BCLabManager.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "battery_types",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    manufacturor = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    material = table.Column<string>(nullable: true),
                    typical_capacity = table.Column<int>(nullable: false),
                    limited_charge_voltage = table.Column<int>(nullable: false),
                    rated_capacity = table.Column<int>(nullable: false),
                    nominal_voltage = table.Column<int>(nullable: false),
                    cutoff_discharge_voltage = table.Column<int>(nullable: false),
                    fully_charged_end_current = table.Column<int>(nullable: false),
                    fully_charged_ending_timeout = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_battery_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chambers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_use_count = table.Column<int>(nullable: false),
                    manufacturor = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    lowest_temperature = table.Column<double>(nullable: false),
                    highest_temperature = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chambers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coefficients",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    temperature = table.Column<double>(nullable: false),
                    slope = table.Column<double>(nullable: false),
                    offset = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_coefficients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lib_fgs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    libfg_model_code = table.Column<int>(nullable: false),
                    libfg_version = table.Column<int>(nullable: false),
                    libfg_sample_file_path = table.Column<string>(nullable: true),
                    libfg_dev_pack_file_path = table.Column<string>(nullable: true),
                    libfg_dll_path = table.Column<string>(nullable: true),
                    is_valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lib_fgs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "program_types",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_program_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipe_templates",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "table_maker_product_types",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_maker_product_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "testers",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    manufacturor = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_testers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "batteries",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_use_count = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    battery_type_id = table.Column<int>(nullable: true),
                    cycle_count = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_batteries", x => x.id);
                    table.ForeignKey(
                        name: "fk_batteries_battery_types_battery_type_id",
                        column: x => x.battery_type_id,
                        principalTable: "battery_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    customer = table.Column<string>(nullable: true),
                    battery_type_id = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    absolute_max_capacity = table.Column<int>(nullable: false),
                    limited_charge_voltage = table.Column<int>(nullable: false),
                    cutoff_discharge_voltage = table.Column<int>(nullable: false),
                    voltage_points = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_battery_types_battery_type_id",
                        column: x => x.battery_type_id,
                        principalTable: "battery_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "step_templates",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    coefficient_id = table.Column<int>(nullable: true),
                    current_input = table.Column<double>(nullable: false),
                    current_unit = table.Column<int>(nullable: false),
                    cut_off_condition_value = table.Column<double>(nullable: false),
                    cut_off_condition_type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_step_templates", x => x.id);
                    table.ForeignKey(
                        name: "fk_step_templates_coefficients_coefficient_id",
                        column: x => x.coefficient_id,
                        principalTable: "coefficients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "channels",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_use_count = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    tester_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channels", x => x.id);
                    table.ForeignKey(
                        name: "fk_channels_testers_tester_id",
                        column: x => x.tester_id,
                        principalTable: "testers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "programs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    order = table.Column<decimal>(nullable: false),
                    requester = table.Column<string>(nullable: true),
                    request_time = table.Column<DateTime>(nullable: false),
                    start_time = table.Column<DateTime>(nullable: false),
                    end_time = table.Column<DateTime>(nullable: false),
                    est = table.Column<DateTime>(nullable: false),
                    eet = table.Column<DateTime>(nullable: false),
                    ed = table.Column<TimeSpan>(nullable: false),
                    type_id = table.Column<int>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    is_invalid = table.Column<bool>(nullable: false),
                    table_file_path = table.Column<string>(nullable: true),
                    project_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_programs", x => x.id);
                    table.ForeignKey(
                        name: "fk_programs_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_programs_program_types_type_id",
                        column: x => x.type_id,
                        principalTable: "program_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_settings",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    design_capacity_mahr = table.Column<int>(nullable: false),
                    limited_charge_voltage_mv = table.Column<int>(nullable: false),
                    fully_charged_end_current_ma = table.Column<int>(nullable: false),
                    fully_charged_ending_time_ms = table.Column<int>(nullable: false),
                    discharge_end_voltage_mv = table.Column<int>(nullable: false),
                    threshold_1st_facc_mv = table.Column<int>(nullable: false),
                    threshold_2nd_facc_mv = table.Column<int>(nullable: false),
                    threshold_3rd_facc_mv = table.Column<int>(nullable: false),
                    threshold_4th_facc_mv = table.Column<int>(nullable: false),
                    initial_ratio_fcc = table.Column<int>(nullable: false),
                    accumulated_capacity_mahr = table.Column<int>(nullable: false),
                    dsg_low_volt_mv = table.Column<int>(nullable: false),
                    dsg_low_temp_01dc = table.Column<int>(nullable: false),
                    initial_soc_start_ocv = table.Column<int>(nullable: false),
                    system_line_impedance = table.Column<int>(nullable: false),
                    is_valid = table.Column<bool>(nullable: false),
                    project_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_settings_projects_project_class_id",
                        column: x => x.project_class_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "table_maker_products",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_path = table.Column<string>(nullable: true),
                    type_id = table.Column<int>(nullable: true),
                    project_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_maker_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_table_maker_products_projects_project_class_id",
                        column: x => x.project_class_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_table_maker_products_table_maker_product_types_type_id",
                        column: x => x.type_id,
                        principalTable: "table_maker_product_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "steps",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    step_template_id = table.Column<int>(nullable: true),
                    loop_label = table.Column<string>(nullable: true),
                    loop_target = table.Column<string>(nullable: true),
                    loop_count = table.Column<int>(nullable: false),
                    compare_mark = table.Column<int>(nullable: false),
                    c_rate = table.Column<double>(nullable: false),
                    order = table.Column<int>(nullable: false),
                    recipe_template_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_steps_recipe_templates_recipe_template_id",
                        column: x => x.recipe_template_id,
                        principalTable: "recipe_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_steps_step_templates_step_template_id",
                        column: x => x.step_template_id,
                        principalTable: "step_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asset_usage_records",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timestamp = table.Column<DateTime>(nullable: false),
                    asset_use_count = table.Column<int>(nullable: false),
                    program_name = table.Column<string>(nullable: true),
                    recipe_name = table.Column<string>(nullable: true),
                    battery_class_id = table.Column<int>(nullable: true),
                    chamber_class_id = table.Column<int>(nullable: true),
                    channel_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_usage_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_usage_records_batteries_battery_class_id",
                        column: x => x.battery_class_id,
                        principalTable: "batteries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_usage_records_chambers_chamber_class_id",
                        column: x => x.chamber_class_id,
                        principalTable: "chambers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_asset_usage_records_channels_channel_class_id",
                        column: x => x.channel_class_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_abandoned = table.Column<bool>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    temperature = table.Column<double>(nullable: false),
                    start_time = table.Column<DateTime>(nullable: false),
                    end_time = table.Column<DateTime>(nullable: false),
                    est = table.Column<DateTime>(nullable: false),
                    eet = table.Column<DateTime>(nullable: false),
                    ed = table.Column<TimeSpan>(nullable: false),
                    program_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipes", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipes_programs_program_class_id",
                        column: x => x.program_class_id,
                        principalTable: "programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "release_packages",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_setting_id = table.Column<int>(nullable: true),
                    lib_fg_id = table.Column<int>(nullable: true),
                    error_rsoc = table.Column<string>(nullable: true),
                    error_tte = table.Column<string>(nullable: true),
                    result_file_path = table.Column<string>(nullable: true),
                    package_file_path = table.Column<string>(nullable: true),
                    emulator_info = table.Column<string>(nullable: true),
                    is_valid = table.Column<bool>(nullable: false),
                    project_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_release_packages", x => x.id);
                    table.ForeignKey(
                        name: "fk_release_packages_projects_project_class_id",
                        column: x => x.project_class_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_release_packages_lib_fgs_lib_fg_id",
                        column: x => x.lib_fg_id,
                        principalTable: "lib_fgs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_release_packages_project_settings_project_setting_id",
                        column: x => x.project_setting_id,
                        principalTable: "project_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "step_runtimes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    step_template_id = table.Column<int>(nullable: true),
                    design_capacity_inm_ah = table.Column<double>(nullable: false),
                    start_time = table.Column<DateTime>(nullable: false),
                    end_time = table.Column<DateTime>(nullable: false),
                    duration = table.Column<TimeSpan>(nullable: false),
                    est = table.Column<DateTime>(nullable: false),
                    eet = table.Column<DateTime>(nullable: false),
                    ed = table.Column<TimeSpan>(nullable: false),
                    order = table.Column<int>(nullable: false),
                    recipe_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_step_runtimes", x => x.id);
                    table.ForeignKey(
                        name: "fk_step_runtimes_recipes_recipe_class_id",
                        column: x => x.recipe_class_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_step_runtimes_step_templates_step_template_id",
                        column: x => x.step_template_id,
                        principalTable: "step_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "test_records",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<int>(nullable: false),
                    battery_type_str = table.Column<string>(nullable: true),
                    battery_str = table.Column<string>(nullable: true),
                    tester_str = table.Column<string>(nullable: true),
                    channel_str = table.Column<string>(nullable: true),
                    chamber_str = table.Column<string>(nullable: true),
                    recipe_str = table.Column<string>(nullable: true),
                    program_str = table.Column<string>(nullable: true),
                    project_str = table.Column<string>(nullable: true),
                    start_time = table.Column<DateTime>(nullable: false),
                    end_time = table.Column<DateTime>(nullable: false),
                    comment = table.Column<string>(nullable: true),
                    new_cycle = table.Column<double>(nullable: false),
                    measurement_gain = table.Column<double>(nullable: false),
                    measurement_offset = table.Column<double>(nullable: false),
                    trace_resistance = table.Column<double>(nullable: false),
                    capacity_difference = table.Column<double>(nullable: false),
                    test_file_path = table.Column<string>(nullable: true),
                    @operator = table.Column<string>(name: "operator", nullable: true),
                    current = table.Column<double>(nullable: false),
                    temperature = table.Column<double>(nullable: false),
                    assigned_battery_id = table.Column<int>(nullable: true),
                    assigned_chamber_id = table.Column<int>(nullable: true),
                    assigned_channel_id = table.Column<int>(nullable: true),
                    recipe_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_records_batteries_assigned_battery_id",
                        column: x => x.assigned_battery_id,
                        principalTable: "batteries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_test_records_chambers_assigned_chamber_id",
                        column: x => x.assigned_chamber_id,
                        principalTable: "chambers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_test_records_channels_assigned_channel_id",
                        column: x => x.assigned_channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_test_records_recipes_recipe_class_id",
                        column: x => x.recipe_class_id,
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "emulator_results",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    test_record_id = table.Column<int>(nullable: true),
                    project_setting_id = table.Column<int>(nullable: true),
                    lib_fg_id = table.Column<int>(nullable: true),
                    em_temperature = table.Column<string>(nullable: true),
                    em_current = table.Column<string>(nullable: true),
                    error_rsoc = table.Column<string>(nullable: true),
                    error_tte = table.Column<string>(nullable: true),
                    log_file_path = table.Column<string>(nullable: true),
                    package_file_path = table.Column<string>(nullable: true),
                    rsoc_png_file_path = table.Column<string>(nullable: true),
                    tte_png_file_path = table.Column<string>(nullable: true),
                    emulator_info = table.Column<string>(nullable: true),
                    is_valid = table.Column<bool>(nullable: false),
                    project_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_emulator_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_emulator_results_projects_project_class_id",
                        column: x => x.project_class_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_emulator_results_lib_fgs_lib_fg_id",
                        column: x => x.lib_fg_id,
                        principalTable: "lib_fgs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_emulator_results_project_settings_project_setting_id",
                        column: x => x.project_setting_id,
                        principalTable: "project_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_emulator_results_test_records_test_record_id",
                        column: x => x.test_record_id,
                        principalTable: "test_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asset_usage_records_battery_class_id",
                table: "asset_usage_records",
                column: "battery_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_usage_records_chamber_class_id",
                table: "asset_usage_records",
                column: "chamber_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_usage_records_channel_class_id",
                table: "asset_usage_records",
                column: "channel_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_batteries_battery_type_id",
                table: "batteries",
                column: "battery_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_channels_tester_id",
                table: "channels",
                column: "tester_id");

            migrationBuilder.CreateIndex(
                name: "ix_emulator_results_project_class_id",
                table: "emulator_results",
                column: "project_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_emulator_results_lib_fg_id",
                table: "emulator_results",
                column: "lib_fg_id");

            migrationBuilder.CreateIndex(
                name: "ix_emulator_results_project_setting_id",
                table: "emulator_results",
                column: "project_setting_id");

            migrationBuilder.CreateIndex(
                name: "ix_emulator_results_test_record_id",
                table: "emulator_results",
                column: "test_record_id");

            migrationBuilder.CreateIndex(
                name: "ix_programs_project_id",
                table: "programs",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_programs_type_id",
                table: "programs",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_settings_project_class_id",
                table: "project_settings",
                column: "project_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_battery_type_id",
                table: "projects",
                column: "battery_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_program_class_id",
                table: "recipes",
                column: "program_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_release_packages_project_class_id",
                table: "release_packages",
                column: "project_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_release_packages_lib_fg_id",
                table: "release_packages",
                column: "lib_fg_id");

            migrationBuilder.CreateIndex(
                name: "ix_release_packages_project_setting_id",
                table: "release_packages",
                column: "project_setting_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_runtimes_recipe_class_id",
                table: "step_runtimes",
                column: "recipe_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_runtimes_step_template_id",
                table: "step_runtimes",
                column: "step_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_step_templates_coefficient_id",
                table: "step_templates",
                column: "coefficient_id");

            migrationBuilder.CreateIndex(
                name: "ix_steps_recipe_template_id",
                table: "steps",
                column: "recipe_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_steps_step_template_id",
                table: "steps",
                column: "step_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_table_maker_products_project_class_id",
                table: "table_maker_products",
                column: "project_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_table_maker_products_type_id",
                table: "table_maker_products",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_records_assigned_battery_id",
                table: "test_records",
                column: "assigned_battery_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_records_assigned_chamber_id",
                table: "test_records",
                column: "assigned_chamber_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_records_assigned_channel_id",
                table: "test_records",
                column: "assigned_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_records_recipe_class_id",
                table: "test_records",
                column: "recipe_class_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asset_usage_records");

            migrationBuilder.DropTable(
                name: "emulator_results");

            migrationBuilder.DropTable(
                name: "release_packages");

            migrationBuilder.DropTable(
                name: "step_runtimes");

            migrationBuilder.DropTable(
                name: "steps");

            migrationBuilder.DropTable(
                name: "table_maker_products");

            migrationBuilder.DropTable(
                name: "test_records");

            migrationBuilder.DropTable(
                name: "lib_fgs");

            migrationBuilder.DropTable(
                name: "project_settings");

            migrationBuilder.DropTable(
                name: "recipe_templates");

            migrationBuilder.DropTable(
                name: "step_templates");

            migrationBuilder.DropTable(
                name: "table_maker_product_types");

            migrationBuilder.DropTable(
                name: "batteries");

            migrationBuilder.DropTable(
                name: "chambers");

            migrationBuilder.DropTable(
                name: "channels");

            migrationBuilder.DropTable(
                name: "recipes");

            migrationBuilder.DropTable(
                name: "coefficients");

            migrationBuilder.DropTable(
                name: "testers");

            migrationBuilder.DropTable(
                name: "programs");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "program_types");

            migrationBuilder.DropTable(
                name: "battery_types");
        }
    }
}
