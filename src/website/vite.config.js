import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';
import fs from 'node:fs';
import path from 'node:path';
import child_process from 'node:child_process';
import { env } from 'node:process';

const baseFolder =
  env.APPDATA && env.APPDATA !== ''
    ? `${env.APPDATA}/ASP.NET/https`
    : `${env.HOME}/.aspnet/https`;

const certificateName = 'localhost';
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  const result = child_process.spawnSync(
    'dotnet',
    ['dev-certs', 'https', '--export-path', certFilePath, '--format', 'Pem', '--no-password'],
    { stdio: 'inherit' }
  );
  if (result.status !== 0) {
    throw new Error('Could not create certificate.');
  }
}

export default defineConfig({
  plugins: [sveltekit()],
  server: {
    host: 'localhost',
    port: 5173,
    strictPort: true,
    https: {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath)
    }
  }
});
