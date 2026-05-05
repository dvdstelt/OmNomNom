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

// Always export. The SDK regenerates the dev cert when the existing
// one is missing required SAN entries (e.g., .NET 10 added
// *.dev.localhost and host.docker.internal). A pre-existing .pem on
// disk would otherwise leave Vite serving a cert that no longer
// matches the current dev cert, with the regenerated one only in
// CurrentUser\My. `--export-path` is fast (~1s) and idempotent.
const exportResult = child_process.spawnSync(
  'dotnet',
  ['dev-certs', 'https', '--export-path', certFilePath, '--format', 'Pem', '--no-password'],
  { stdio: 'inherit' }
);
if (exportResult.status !== 0) {
  throw new Error('Could not create certificate.');
}

// Always trust. When the SDK regenerates the dev cert during export
// it lands in CurrentUser\My but NOT in CurrentUser\Root, leaving Vite
// serving an untrusted cert. `--trust` is idempotent: a fast no-op
// when the active cert is already trusted, otherwise pops one UAC
// prompt on Windows. We deliberately don't gate on `--check --trust`,
// because that just verifies SOME dev cert is trusted, not the one we
// just exported, and would silently let an untrusted active cert
// through. Failure here isn't fatal: browsers can still click through;
// the warning steers Linux users toward platform-specific trust.
const trustResult = child_process.spawnSync(
  'dotnet',
  ['dev-certs', 'https', '--trust'],
  { stdio: 'inherit' }
);
if (trustResult.status !== 0) {
  console.warn(
    '[vite] dotnet dev-certs https --trust did not complete cleanly. ' +
    'Browsers will still work via a click-through, but mail clients ' +
    'and the BackOffice may reject the cert with UntrustedRoot. ' +
    'On Linux, run the platform-specific trust command manually.'
  );
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
