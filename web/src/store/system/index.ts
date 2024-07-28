import { create } from "zustand";
import { ThemeMode } from "antd-style";

interface SystemState {
    theme: ThemeMode;
    setTheme: (theme: ThemeMode) => void;
}

const systemStore = create<SystemState>((set) => ({
    theme: localStorage.getItem("theme") as ThemeMode || "auto",
    setTheme: (theme) => set({ theme }),
}));

export default systemStore;