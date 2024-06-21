from pyecharts import options as opts
from pyecharts.charts import Bar, Scatter3D, Line3D
from pyecharts.commons.utils import JsCode
from pyecharts.globals import ThemeType


def plot_avg_velocity(data):
    # Create a bar chart instance
    bar = Bar(init_opts=opts.InitOpts(theme=ThemeType.MACARONS))  # 使用更动人的主题

    # Extract satellite IDs and their respective average velocities
    satellites = [key for key in data if key.startswith('Satellite_')]
    avg_velocities = [data[sat][2]['avg_Velocity'] for sat in satellites]  # 正确索引第二个字典

    # Add data to the bar chart
    bar.add_xaxis(satellites)
    bar.add_yaxis("Average Velocity", avg_velocities,
                  itemstyle_opts=opts.ItemStyleOpts(color="#3398DB"))  # 设置单一颜色

    # Set global options
    bar.set_global_opts(
        title_opts=opts.TitleOpts(title="Average Velocity of Satellites", subtitle="Visualization of Satellite Data",
                                  title_textstyle_opts=opts.TextStyleOpts(color='darkblue', font_size=16)),
        yaxis_opts=opts.AxisOpts(name="Velocity", axislabel_opts=opts.LabelOpts(font_size=12)),
        xaxis_opts=opts.AxisOpts(name="Satellite", axislabel_opts=opts.LabelOpts(rotate=45, font_size=10)),
        toolbox_opts=opts.ToolboxOpts(is_show=True),
    )

    # Enhance the style with a gradient fill
    bar.set_series_opts(
        label_opts=opts.LabelOpts(is_show=False),
    )

    # Render the chart
    bar.render()


def plot_avg_distance(data):
    # Create a bar chart instance
    bar = Bar(init_opts=opts.InitOpts(theme=ThemeType.MACARONS))  # 使用更动人的主题

    # Extract satellite IDs and their respective average velocities
    satellites = [key for key in data if key.startswith('Satellite_')]
    avg_velocities = [data[sat][1]['avg_Distance'] for sat in satellites]  # 正确索引第二个字典

    # Add data to the bar chart
    bar.add_xaxis(satellites)
    bar.add_yaxis("Average Distance", avg_velocities,
                  itemstyle_opts=opts.ItemStyleOpts(color="#3398DB"))  # 设置单一颜色

    # Set global options
    bar.set_global_opts(
        title_opts=opts.TitleOpts(title="Average Distance of Satellites", subtitle="Visualization of Satellite Data",
                                  title_textstyle_opts=opts.TextStyleOpts(color='darkblue', font_size=16)),
        yaxis_opts=opts.AxisOpts(name="Distance", axislabel_opts=opts.LabelOpts(font_size=12)),
        xaxis_opts=opts.AxisOpts(name="Satellite", axislabel_opts=opts.LabelOpts(rotate=45, font_size=10)),
        toolbox_opts=opts.ToolboxOpts(is_show=True),
    )

    # Enhance the style with a gradient fill
    bar.set_series_opts(
        label_opts=opts.LabelOpts(is_show=False),
    )

    # Render the chart
    bar.render()


def plot_3d_trajectory(data):
    # Create a 3D line chart instance
    line3d = Line3D(init_opts=opts.InitOpts(theme=ThemeType.WESTEROS, width="1000px", height="1000px"))

    # Extract satellite IDs
    satellites = [key for key in data if key.startswith('Satellite_')]

    # Iterate through each satellite to add their XYZ data to the chart
    for sat in satellites:
        x = data[sat][3]["AllXValues"]
        y = data[sat][4]["AllYValues"]
        z = data[sat][5]["AllZValues"]
        points = list(zip(x, y, z))
        line3d.add(sat, points, grid3d_opts=opts.Grid3DOpts(width=300, height=300, depth=300), )

    # Set global options
    line3d.set_global_opts(
        title_opts=opts.TitleOpts(title="3D Trajectory of Satellites")
    )

    # Render the chart
    line3d.render()  # Adjust according to your environment
    return line3d
