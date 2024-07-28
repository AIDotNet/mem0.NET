import AppTheme from "./AppTheme";
import StyleRegistry from "./StyleRegistry";
import { ConfigProvider } from 'antd';
import dayjs from 'dayjs';
import 'dayjs/locale/zh-cn';
dayjs.locale('zh-cn');

const Layout = ({
    children
}: any) => {
    return (
        <AppTheme>
            <ConfigProvider
                // 默认语言
                locale={{
                    locale: 'zh-cn',
                    Empty: {
                        description: '暂无数据'
                    },
                    global: {
                        placeholder: '请选择'
                    },
                    Select: {
                        notFoundContent: '无匹配结果'
                    },
                }}
            >
                <StyleRegistry>
                    {children}
                </StyleRegistry>
            </ConfigProvider>
        </AppTheme>
    );
});

export default Layout;