# Getting into the server, after the handover

This page is for the WailSalutem Foundation and for whoever looks after the server of Connect after the project ends in January 2027. It says how a person gets into the server, what to do when nobody with access is left, and what changes at the handover. The technical details are in part 10 of [onboarding.md](../team/onboarding.md).

No password, key or server number is on this page. Those pass in person or through a password manager, never by mail, chat, Git or GitLab.

## The server in short

The server belongs to the foundation. It is a Strato VPS in a data centre in Germany, managed through the foundation's own Strato customer login.

The machine, its address and the programs on it are listed in part 10 of [onboarding.md](../team/onboarding.md), section The machine.

What kind of Strato server it is, checked on the server on 10 October 2026:

- It is a virtual server on KVM (`systemd-detect-virt` answers `kvm`). Strato runs its newer servers of the classes VC, PA-C and VDS on KVM, and manages all three directly in the customer login, without a separate server login (Strato, n.d.-c).
- It has 4 cores, 8 GB of memory and a disk of 240 GB. That is the package Strato sells today as VPS L (Strato, n.d.-d).
- The main part of the disk is `/dev/vda1`. The steps of the rescue system below use exactly that name.

## During the project

- Only the five team members log in to the server, each with their own key. Password login is off.
- The client does not log in to the server and does not work with the code. He has the Strato customer login, where he can reset the whole server.
- The server holds no real data during the project, so a reset loses nothing that matters.
- In the last sprint, when the project is done, the team switches password login back on, with step 6 of At the handover.

## The normal way in: a personal login with a key

Every person who works on the server has a login of their own, and that login opens only with a key. A key is a small pair of files on a laptop. The private half stays on the laptop like a house key, and the public half is put on the server like the lock that fits it.

During the project, a login with a password over the internet is refused, for everyone.

Why: the server can be reached from the whole internet, and computers try passwords on servers like this all day. A key cannot be guessed. The team decided this for security (issue #237), and the foundation checks it in issue #10: "I see that the server refuses a login with a password".

How a new person gets in:

1. They make a key on their own laptop and send the public half, the line that starts with `ssh-ed25519`. Section Logging in of part 10 shows how.
2. Someone who already has access adds them with the steps of section New team member.
3. They log in with `ssh <their name>@<the address of the server>`.

Today all five team members have access, and each of them can add a new person.

## Without a key: the rescue system of Strato

When nobody with access is left, the foundation can always get in through its Strato customer login.

The server restarts for this, so the site is offline for a few minutes. These steps follow Strato's own guides for this kind of server (Strato, n.d.-a, n.d.-b).

1. Log in to the Strato customer login (Kunden-Login) with the customer number and password of the foundation. In the package overview, click the name of the server. The page of the server opens, with buttons such as **Neustarten**, **Neuinstallation** and **Rettungssystem starten**.
   - Never click **Neuinstallation**. It puts a new system on the server, and everything on it now is gone.
2. Click **Rettungssystem starten** (start the rescue system) and confirm.
3. When the rescue system has started, click **VNC-Konsole öffnen** (open the console). A window with the server's screen opens in the browser, without a password. Press Enter at the first choice, choose US English and press Enter again.
4. Open the terminal behind the window and type these four lines. They open the disk of the server, so the next commands change the real server:

   ```
   sudo mkdir /media/vda1
   sudo mount /dev/vda1 /media/vda1
   cd /media/vda1
   sudo chroot . /bin/bash
   ```

5. Give a person access again with the steps of section New team member in part 10, without the word `sudo` in front, because you are root here already. Add a line for them in `/etc/sudoers.d/20-connect-admins` if they must manage the server.
6. Type `exit`, go back to the page of the server in the Strato customer login and click **Rettungssystem stoppen** (stop the rescue system). The server starts normally, and the person logs in with their key.

## The root password

The server's administrator account, root, has a password. It came with the server, and the team received the first login from the foundation on 2 October 2026.

Root cannot log in over the internet. The password only works on a console of the server.

At the handover the foundation sets a new root password, so that nobody from the team knows it any more.

In the rescue system, after step 4 above, type `passwd` and enter the new password twice (Strato, n.d.-a). Then stop the rescue system as in step 6.

## At the handover

The handover session gets a date before the last sprint review, see [deliveryPlan.md](../project/deliveryPlan.md). In that session:

1. The foundation logs in to the Strato customer login, clicks the server in the package overview and finds the button Rettungssystem starten, without clicking it.
   - TODO team at the handover: write here the package name the customer login shows for the server, with the date. A name with VC or VDS in it confirms the class of The server in short.
2. The foundation sets a new root password, see The root password.
3. The foundation decides per team member whether they keep access. Whoever leaves is removed with section Someone leaves in part 10, including their line in the sudo file.
4. When GitLab no longer puts new versions on the server, the key of the user deploy is removed from `/home/deploy/.ssh/authorized_keys`, and the variables SSH_PRIVATE_KEY, SSH_KNOWN_HOSTS and TEST_SERVER_HOST are deleted in GitLab.
5. The person who looks after the server next gets a login of their own with their key, through section New team member.
6. The team switches the login with a password back on, the way the server came: `PasswordAuthentication yes` and `PermitRootLogin yes` in `/etc/ssh/sshd_config.d/10-connect.conf`, checked with `sudo sshd -t` before `sudo systemctl reload ssh`. The foundation then logs in with the new root password of step 2.

During the project a login with a password over the internet stays off, as issue #10 asks. It comes back on with step 6, once the project is done.

## References

Strato. (n.d.-a). *Das Rettungssystem Ihres STRATO Linux VPS/VDS* (FAQ 3793). Retrieved October 10, 2026, from https://www.strato.de/faq/server/das-rettungssystem-ihres-strato-linux-vps-vds/

Strato. (n.d.-b). *Die ersten Schritte mit Ihrem Linux VPS/VDS* (FAQ 3941). Retrieved October 10, 2026, from https://www.strato.de/faq/server/die-ersten-schritte-mit-ihrem-linux-vps-vds/

Strato. (n.d.-c). *Erste Schritte mit Ihrem STRATO-Server* (FAQ 260). Retrieved October 10, 2026, from https://www.strato.de/faq/server/erste-schritte-mit-ihrem-strato-server/

Strato. (n.d.-d). *VPS Server: Maximale Leistung günstig mieten*. Retrieved October 10, 2026, from https://www.strato.de/server/vps/

## AI use declaration

AI was used as a writing support tool while working on this document. We used AI to improve the formulation of sentences, the structure of wording, readability, and clarity of text that we had written. The content itself, including our experiences, reflections, learning goals, decisions, self-assessments, and conclusions, is our own. We reviewed the suggested changes and decided as a team which improvements to include.
