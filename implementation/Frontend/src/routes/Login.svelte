<script lang="ts">
  import { Button, Card, TextField } from "attractions";
  import { handleLogin } from "../lib/auth";
  import { redirectIfAuthenticated, type Route } from "../lib/routes";
  import { push } from "svelte-spa-router";
  import topmarkLogo from "../assets/topmark.png";

  redirectIfAuthenticated();

  const registerLink: Route = "/register";

  let email = "";
  let password = "";
  let hasErrors = false;

  async function tryLogin() {
    const wasSuccess = await handleLogin(email, password);
    if (wasSuccess) {
      redirectIfAuthenticated();
    } else {
      hasErrors = true;
    }
  }
</script>

<div class="login-container">
  <img src={topmarkLogo} alt="Topmark Logo" />

  <h1>Login</h1>
  <Card>
    <TextField
      type="email"
      autofocus
      outline
      bind:value={email}
      label="Email"
      withItem
      style="margin-bottom: 20px"
    />
    <TextField
      outline
      type="password"
      bind:value={password}
      label="Password"
      withItem
    />
    {#if hasErrors}
      <p class="danger">Fejl under login. Prøv igen</p>
    {/if}
  </Card>
  <Button style="margin-top: 20px" on:click={tryLogin} filled>Login</Button>
  <Button style="margin-top: 20px" outline on:click={() => push(registerLink)}>
    Register
  </Button>
</div>

<style>
  .login-container {
    display: flex;
    margin: 2cm;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }
</style>
