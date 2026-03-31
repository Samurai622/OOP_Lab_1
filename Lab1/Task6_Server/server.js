import express from 'express';
import cors from 'cors';
import bodyParser from 'body-parser';
import { spawn } from "child_process";
import path from "path";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const app = express();
app.use(cors());
app.use(bodyParser.json({ limit: "10mb" }));

// Абсолютний шлях до Lab1.dll (EXE-проєкт)
const DLL_PATH = path.join(
    __dirname,
    "..",
    "Lab1_Task6_CLI",
    "bin",
    "Debug",
    "net10.0",
    "Lab1_Task6_CLI.dll"
);

console.log("DLL PATH:", DLL_PATH);

app.post("/api/check", (req, res) => {
    const { code, lang, algo } = req.body;

    const child = spawn("dotnet", [
    DLL_PATH,
    "--",
    "--cli",
    lang.replace(" ", "_"),
    algo.replace(" ", "_")
    ]);

    let out = "";
    let err = "";

    child.stdout.on("data", data => out += data.toString());
    child.stderr.on("data", data => err += data.toString());

    child.on("close", (exitCode) => {
        if (exitCode !== 0) {
            return res.json({ success: false, message: err });
        }
        return res.json({ success: true, message: out });
    });

    child.stdin.write(code);
    child.stdin.end();
});
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`SERVER RUNNING ON PORT ${PORT}`);
});